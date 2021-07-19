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
        private readonly Dictionary<object, ServiceMethod[]> methods =
            new Dictionary<object, ServiceMethod[]>();
        private readonly TypeReaderWriterManager readerWriterManager;
        private readonly object invokeLock;
        
        /// <summary>
        ///     Logger
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        ///     Creates a new <see cref="Host"/> instance
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        protected Host(ILogger logger = null)
        {
            Logger = logger ?? new NullLogger();

            readerWriterManager = new TypeReaderWriterManager();
            invokeLock = new object();
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
        /// </summary>
        /// <param name="readStream">The read stream</param>
        /// <param name="writeStream">The write stream</param>
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

            BufferedReader reader = new BufferedReader(readStream);
            BufferedWriter writer = new BufferedWriter(writeStream);
            
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
                    writer.WriteByte((byte)MessageResponse.NoMethodFound);
                    writer.Flush();
                    Logger.Warn("Client sent an invalid method request.");
                    return;
                }
            
                //Now we read the parameters
                int paramsCount = reader.ReadInt();
                object[] parameters = new object[paramsCount];
                for (int i = 0; i < paramsCount; i++)
                {
                    //Read the type
                    string type = reader.ReadString();
                    ITypeReadWriter typeRead = readerWriterManager.GetType(type);
                    if (typeRead == null)
                    {
                        writer.WriteByte((byte)MessageResponse.ExecuteFailNoTypeReader);
                        writer.Flush();
                        Logger.Error($"The client sent a method with a parameter type of '{type}' of which I don't have a type reader for some reason!");
                        return;
                    }

                    try
                    {
                        parameters[i] = typeRead.Read(reader);
                    }
                    catch (Exception ex)
                    {
                        writer.WriteByte((byte)MessageResponse.ExecuteTypeReadWriteFail);
                        writer.WriteString(ex.Message);
                        writer.Flush();
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
                    writer.WriteByte((byte)MessageResponse.ExecuteInvokeFailException);
                    writer.WriteString(ex.Message);
                    writer.Flush();
                    Logger.Error($"Method invoke failed! {ex}");
                    return;
                }
            
                //If the method doesn't return void, write it back
                if (!method.IsReturnVoid)
                {
                    ITypeReadWriter typeWriter = readerWriterManager.GetType(method.ReturnTypeName);
                    if (typeWriter == null)
                    {
                        writer.WriteByte((byte)MessageResponse.ExecuteFailNoTypeReader);
                        writer.Flush();
                        Logger.Error($"The client sent a method with a return type of '{method.ReturnTypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }
                
                    writer.WriteByte((byte)MessageResponse.ExecutedSuccessful);
                    try
                    {
                        typeWriter.Write(writer, methodReturn);
                        writer.Flush();
                        return;
                    }
                    catch (Exception ex)
                    {
                        writer.WriteByte((byte)MessageResponse.ExecuteTypeReadWriteFail);
                        writer.WriteString(ex.Message);
                        writer.Flush();
                        Logger.Error($"Parsing return type of '{method.ReturnTypeName}' failed for some reason! {ex}");
                        return;
                    }
                }

                writer.WriteByte((byte)MessageResponse.ExecutedSuccessful);
                writer.Flush();
            }
        }
    }
}