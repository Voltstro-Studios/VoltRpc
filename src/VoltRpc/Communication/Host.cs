using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using VoltRpc.IO;
using VoltRpc.Logging;
using VoltRpc.Services;
using VoltRpc.Types;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Host" /> receives and responds to a <see cref="Client" />'s requests
    /// </summary>
    public abstract class Host : IDisposable
    {
        /// <summary>
        ///     The default size of the buffers
        /// </summary>
        public const int DefaultBufferSize = 8000;

        /// <summary>
        ///     This size of the buffer
        /// </summary>
        protected readonly int BufferSize;

        private readonly object invokeLock;

        /// <summary>
        ///     Logger
        /// </summary>
        protected readonly ILogger Logger;

        private readonly List<HostService> services = new List<HostService>();

        /// <summary>
        ///     Creates a new <see cref="Host" /> instance
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" /> to use</param>
        /// <param name="bufferSize">The initial size of the buffers</param>
        /// <exception cref="ArgumentOutOfRangeException">Will throw if the buffer size is less then 16</exception>
        protected Host(ILogger logger = null, int bufferSize = DefaultBufferSize)
        {
            if (bufferSize < 16)
                throw new ArgumentOutOfRangeException(nameof(bufferSize),
                    "The buffer needs to be larger then 15 bytes!");

            Logger = logger ?? new NullLogger();

            TypeReaderWriterManager = new TypeReaderWriterManager();
            invokeLock = new object();

            BufferSize = bufferSize;
        }

        /// <summary>
        ///     The <see cref="Types.TypeReaderWriterManager" /> for <see cref="Host" />
        /// </summary>
        public TypeReaderWriterManager TypeReaderWriterManager { get; }

        /// <summary>
        ///     Count of number of connections
        /// </summary>
        public int ConnectionCount { get; protected set; }
        
        /// <summary>
        ///     What is the maximum amount of connections
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if value is 0 or less</exception>
        public int MaxConnectionsCount
        {
            get => maxConnectionsCount;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The value needs to be larger then 0!");

                maxConnectionsCount = value;
            }
        }

        private int maxConnectionsCount = 16;
        
        /// <summary>
        ///     Is the <see cref="Host"/> running?
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        ///     Hides the stacktrace from the client when an <see cref="Exception" /> is thrown
        /// </summary>
        public bool HideStacktrace { get; set; }

        /// <summary>
        ///     Starts the <see cref="Host" /> to listen for requests
        /// </summary>
        public abstract Task StartListening();

        /// <summary>
        ///     Adds a service to this <see cref="Host" />
        /// </summary>
        /// <param name="service">The service <see cref="object" /> to add</param>
        /// <typeparam name="T">The service type</typeparam>
        /// <exception cref="ArgumentException">Thrown if the service has already been added</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if T is not an interface</exception>
#if NET6_0_OR_GREATER
        [RequiresUnreferencedCode("Use AddService(Type serviceType, object serviceObject) instead.")]
#endif
        public void AddService<T>(T service)
            where T : class
        {
            AddService(typeof(T), service);
        }

        /// <summary>
        ///     Adds a service to this <see cref="Host" />
        /// </summary>
        /// <param name="serviceType">The <see cref="Type"/> of a service</param>
        /// <param name="serviceObject">The actual service <see cref="object"/> itself</param>
        /// <exception cref="ArgumentException">Thrown if the service has already been added</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if serviceType is not an interface</exception>
#if NET6_0_OR_GREATER
        public void AddService([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type serviceType, 
#else
        public void AddService(Type serviceType, 
#endif
            object serviceObject)
        {
            if (!serviceType.IsInterface)
                throw new ArgumentOutOfRangeException(nameof(serviceType), "Service Type is not an interface!");
            
            if (services.Exists(x => x.InterfaceObject == serviceObject 
                                     || x.InterfaceName == serviceType.FullName))
                throw new ArgumentException("The service already exists!", nameof(serviceType));
            
            services.Add(new HostService
            {
                InterfaceName = serviceType.FullName,
                InterfaceObject = serviceObject,
                ServiceMethods = ServiceHelper.GetAllServiceMethods(serviceType)
            });
        }

        /// <summary>
        ///     Processes a request from a client
        ///     <para>
        ///         This override will automatically create the <see cref="BufferedReader" /> and <see cref="BufferedWriter" /> for
        ///         you
        ///         then call <see cref="ProcessRequest(BufferedReader,BufferedWriter)" />.
        ///         <para>This is the preferred process request method to call.</para>
        ///     </para>
        /// </summary>
        /// <param name="readStream">The <see cref="Stream" /> to read from</param>
        /// <param name="writeStream">The <see cref="Stream" /> to write to</param>
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
        ///         You should only call this if you need to provide a custom <see cref="BufferedReader" /> and/or
        ///         <see cref="BufferedWriter" />.
        ///         For example you are using a <see cref="Stream" /> that needs <see cref="Stream.Position" />.
        ///     </para>
        /// </summary>
        /// <param name="reader">The <see cref="BufferedReader" /> to read from</param>
        /// <param name="writer">The <see cref="BufferedWriter" /> to write to</param>
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
                foreach (HostService service in services)
                {
                    if (method != null)
                        break;

                    foreach (ServiceMethod serviceMethod in service.ServiceMethods)
                        if (serviceMethod.MethodName == methodName)
                        {
                            obj = service.InterfaceObject;
                            method = serviceMethod;
                            break;
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
                    ServiceMethodParameter parameter = method.Parameters[i];

                    //If it is a out, the we just set it to null and don't read (as nothing is sent for outs)
                    if (parameter.IsOut)
                    {
                        parameters[i] = null;
                        continue;
                    }

                    //Get the type reader
                    ITypeReadWriter typeRead = TypeReaderWriterManager.GetType(parameter.ParameterTypeName);
                    if (typeRead == null)
                    {
                        WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                        Logger.Error(
                            $"The client sent a method with a parameter type of '{parameter.ParameterTypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }

                    try
                    {
                        parameters[i] = typeRead.Read(reader);
                    }
                    catch (Exception ex)
                    {
                        WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message,
                            ex.InnerException?.StackTrace);
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
                    WriteError(writer, MessageResponse.ExecuteInvokeFailException, ex.Message,
                        ex.InnerException?.StackTrace);
                    Logger.Error($"Method invoke failed! {ex}");
                    return;
                }

                writer.WriteByte((byte) MessageResponse.ExecutedSuccessful);

                //If the method doesn't return void, write it back
                if (!method.IsReturnVoid)
                {
                    ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(method.ReturnTypeName);
                    if (typeWriter == null)
                    {
                        WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                        Logger.Error(
                            $"The client sent a method with a return type of '{method.ReturnTypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }

                    try
                    {
                        typeWriter.Write(writer, methodReturn);
                    }
                    catch (Exception ex)
                    {
                        WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message,
                            ex.InnerException?.StackTrace);
                        Logger.Error($"Parsing return type of '{method.ReturnTypeName}' failed for some reason! {ex}");
                        return;
                    }
                }

                //If we have any out or ref types
                if (method.ContainsRefOrOutParameters)
                    for (int i = 0; i < method.Parameters.Length; i++)
                    {
                        ServiceMethodParameter parameter = method.Parameters[i];
                        if (!parameter.IsOut && !parameter.IsRef)
                            continue;

                        ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(parameter.ParameterTypeName);
                        if (typeWriter == null)
                        {
                            WriteError(writer, MessageResponse.ExecuteFailNoTypeReader);
                            Logger.Error(
                                $"The client sent a method with a parameter ref/out type of '{parameter.ParameterTypeName}' of which I don't have a type reader for some reason!");
                            return;
                        }

                        try
                        {
                            typeWriter.Write(writer, parameters[i]);
                        }
                        catch (Exception ex)
                        {
                            WriteError(writer, MessageResponse.ExecuteTypeReadWriteFail, ex.Message,
                                ex.InnerException?.StackTrace);
                            Logger.Error(
                                $"Parsing return type of '{method.ReturnTypeName}' failed for some reason! {ex}");
                            return;
                        }
                    }

                writer.Flush();
            }
        }

        private void WriteError(BufferedWriter writer, MessageResponse message, string error = null,
            string stackTrace = null)
        {
            writer.Reset();
            writer.WriteByte((byte) message);
            switch (message)
            {
                case MessageResponse.NoMethodFound:
                case MessageResponse.ExecutedSuccessful:
                case MessageResponse.ExecuteFailNoTypeReader:
                    break;
                case MessageResponse.ExecuteTypeReadWriteFail:
                case MessageResponse.ExecuteInvokeFailException:
                    writer.WriteString(error);
                    writer.WriteString(HideStacktrace ? null : stackTrace);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message), message, null);
            }

            writer.Flush();
        }

        #region Destory

        /// <summary>
        ///     Destroys the <see cref="Host" /> instance
        /// </summary>
        public virtual void Dispose()
        {
            IsRunning = false;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}