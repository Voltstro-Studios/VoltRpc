using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VoltRpc.IO;
using VoltRpc.Logging;
using VoltRpc.Proxy;
using VoltRpc.Types;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Host"/> receives and responds to a <see cref="Client"/>'s requests
    /// </summary>
    public abstract class Host : IDisposable
    {
        //TODO: Store interface name
        private readonly Dictionary<object, ServiceMethod[]> methods =
            new Dictionary<object, ServiceMethod[]>();
        private readonly object invokeLock;

        /// <summary>
        ///     This size of the buffer
        /// </summary>
        protected readonly int BufferSize;
        
        /// <summary>
        ///     Logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        ///     The <see cref="Types.TypeReaderWriterManager"/> for <see cref="Host"/>
        /// </summary>
        public TypeReaderWriterManager ReaderWriterManager
        {
            get;
        }

        /// <summary>
        ///     Creates a new <see cref="Host"/> instance
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use</param>
        /// <param name="bufferSize">The initial size of the buffers</param>
        /// <exception cref="ArgumentOutOfRangeException">Will throw if the buffer size is less then 16</exception>
        protected Host(ILogger logger = null, int bufferSize = 8000)
        {
            if (bufferSize < 16)
                throw new ArgumentOutOfRangeException(nameof(bufferSize),
                    "The buffer needs to be larger then 15 bytes!");
            
            Logger = logger ?? new NullLogger();

            ReaderWriterManager = new TypeReaderWriterManager();
            invokeLock = new object();
            
            this.BufferSize = bufferSize;
        }
        
        /// <summary>
        ///     Starts the <see cref="Host"/> to listen for requests
        /// </summary>
        public abstract Task StartListening();

        /// <summary>
        ///     Destroys the <see cref="Host"/> instance
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        ///     Adds a service to this <see cref="Host"/>
        /// </summary>
        /// <param name="service">The service <see cref="object"/> to add</param>
        /// <typeparam name="T">The service type</typeparam>
        /// <exception cref="ArgumentException">Thrown if the service has already been added</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if T is not an interface</exception>
        public void AddService<T>(T service) 
            where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentOutOfRangeException(nameof(T), "T is not an interface!");
            
            if (methods.ContainsKey(service))
                throw new ArgumentException("The service already exists!", nameof(service));

            ServiceMethod[] serviceMethods = ServiceHelper.GetAllServiceMethods<T>();
            methods.Add(service, serviceMethods);
        }
        
        /// <summary>
        ///     Processes a request from a client
        ///     <para>
        ///         This override will automatically create the <see cref="BufferedReader"/> and <see cref="BufferedWriter"/> for you
        ///         then call <see cref="ProcessRequest(BufferedReader,BufferedWriter)"/>.
        ///         <para>This is the preferred process request method to call.</para>
        ///     </para>
        /// </summary>
        /// <param name="readStream">The <see cref="Stream"/> to read from</param>
        /// <param name="writeStream">The <see cref="Stream"/> to write to</param>
        /// <exception cref="ArgumentNullException">Thrown if either provide stream is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if we can't read or write to the respected stream</exception>
        protected void ProcessRequest(Stream readStream, Stream writeStream)
        {
            if (readStream == null)
                throw new ArgumentNullException(nameof(readStream));
            
            if (writeStream == null)
                throw new ArgumentNullException(nameof(writeStream));

            if (!readStream.CanRead)
                throw new ArgumentOutOfRangeException(nameof(readStream), "The read stream cannot be read to!");
            
            if (!writeStream.CanWrite)
                throw new ArgumentOutOfRangeException(nameof(writeStream), "The write stream cannot be wrote to!");

            BufferedReader reader = new BufferedReader(readStream, BufferSize);
            BufferedWriter writer = new BufferedWriter(writeStream, BufferSize);
            
            ProcessRequest(reader, writer);
        }

        /// <summary>
        ///     Processes a request from a client
        ///     <para>
        ///         You should only call this if you need to provide a custom <see cref="BufferedReader"/> and/or <see cref="BufferedWriter"/>.
        ///         For example you are using a <see cref="Stream"/> that needs <see cref="Stream.Position"/>.
        ///     </para>
        /// </summary>
        /// <param name="reader">The <see cref="BufferedReader"/> to read from</param>
        /// <param name="writer">The <see cref="BufferedWriter"/> to write to</param>
        /// <exception cref="ArgumentNullException">Thrown if either buffer is null</exception>
        protected void ProcessRequest(BufferedReader reader, BufferedWriter writer)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            
            bool doContinue = true;
            do
            {
                try
                {
                    MessageType messageType = (MessageType) reader.ReadByte();
                    switch (messageType)
                    {
                        case MessageType.InvokeMethod:
                            ProcessInvokeMethod(reader, writer);
                            break;
                        case MessageType.Shutdown:
                            doContinue = false;
                            break;
                    }
                }
                catch (IOException)
                {
                    //A timeout most likely occured
                    doContinue = false;
                }
            } while (doContinue);
            
            reader.Dispose();
            writer.Dispose();
        }

        private void ProcessInvokeMethod(BufferedReader reader, BufferedWriter writer)
        {
            lock (invokeLock)
            {
                //First, read the method
                string methodName = reader.ReadString();

                object obj = null;
                ServiceMethod method = null;
                foreach (KeyValuePair<object, ServiceMethod[]> service in methods)
                {
                    if(method != null)
                        break;
                
                    foreach (ServiceMethod serviceMethod in service.Value)
                    {
                        if (serviceMethod.MethodName == methodName)
                        {
                            obj = service.Key;
                            method = serviceMethod;
                        }
                    }
                }

                //No method was found
                if (method == null)
                {
                    WriteError(writer, MessageResponse.NoMethodFound);
                    Logger.Warn("Client sent an invalid method request.");
                    return;
                }
            
                //Now we read the parameters
                int paramsCount = method.Parameters.Length;
                object[] parameters = new object[paramsCount];
                for (int i = 0; i < paramsCount; i++)
                {
                    Parameter parameter = method.Parameters[i];
                    
                    //If it is a out, the we just set it to null and don't read (as nothing is sent for outs)
                    if (parameter.IsOut)
                    {
                        parameters[i] = null;
                        continue;
                    }
                    
                    //Get the type reader
                    ITypeReadWriter typeRead = ReaderWriterManager.GetType(parameter.ParameterTypeName);
                    if (typeRead == null)
                    {
                        WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                        Logger.Error($"The client sent a method with a parameter type of '{parameter.ParameterTypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }

                    try
                    {
                        parameters[i] = typeRead.Read(reader);
                    }
                    catch (Exception ex)
                    {
                        WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message, ex.InnerException?.StackTrace);
                        Logger.Warn("Client sent invalid parameter data.");
                        return;
                    }
                }

                //Invoke the method
                object methodReturn;
                try
                {
                    methodReturn = method.MethodInfo.Invoke(obj, parameters);
                }
                catch (Exception ex)
                {
                    WriteError(writer, MessageResponse.ExecuteInvokeFailException, ex.Message, ex.InnerException?.StackTrace);
                    Logger.Error($"Method invoke failed! {ex}");
                    return;
                }
                
                writer.WriteByte((byte)MessageResponse.ExecutedSuccessful);

                //If the method doesn't return void, write it back
                if (!method.IsReturnVoid)
                {
                    ITypeReadWriter typeWriter = ReaderWriterManager.GetType(method.ReturnTypeName);
                    if (typeWriter == null)
                    {
                        WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                        Logger.Error($"The client sent a method with a return type of '{method.ReturnTypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }
                    
                    try
                    {
                        typeWriter.Write(writer, methodReturn);
                    }
                    catch (Exception ex)
                    {
                        WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message, ex.InnerException?.StackTrace);
                        Logger.Error($"Parsing return type of '{method.ReturnTypeName}' failed for some reason! {ex}");
                        return;
                    }
                }

                //If we have any out or ref types
                if (method.ContainsRefOrOutParameters)
                {
                    for (int i = 0; i < method.Parameters.Length; i++)
                    {
                        Parameter parameter = method.Parameters[i];
                        if(!parameter.IsOut && !parameter.IsRef)
                            continue;
                        
                        ITypeReadWriter typeWriter = ReaderWriterManager.GetType(parameter.ParameterTypeName);
                        if (typeWriter == null)
                        {
                            WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                            Logger.Error($"The client sent a method with a parameter ref/out type of '{parameter.ParameterTypeName}' of which I don't have a type reader for some reason!");
                            return;
                        }
                        
                        try
                        {
                            typeWriter.Write(writer, parameters[i]);
                        }
                        catch (Exception ex)
                        {
                            WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message, ex.InnerException?.StackTrace);
                            Logger.Error($"Parsing return type of '{method.ReturnTypeName}' failed for some reason! {ex}");
                            return;
                        }
                    }
                }

                writer.Flush();
            }
        }

        private void WriteError(BufferedWriter writer, MessageResponse message, string error = null, string stackTrace = null)
        {
            writer.Reset();
            writer.WriteByte((byte)message);
            switch (message)
            {
                case MessageResponse.NoMethodFound:
                    break;
                case MessageResponse.ExecutedSuccessful:
                    break;
                case MessageResponse.ExecuteFailNoTypeReader:
                    break;
                case MessageResponse.ExecuteTypeReadWriteFail:
                case MessageResponse.ExecuteInvokeFailException:
                    writer.WriteString(error);
                    writer.WriteString(stackTrace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }
            writer.Flush();
        }
    }
}