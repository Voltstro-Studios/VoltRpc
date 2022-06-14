using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VoltRpc.Communication.Protocol;
using VoltRpc.IO;
using VoltRpc.Logging;
using VoltRpc.Services;
using VoltRpc.Types;

#nullable enable
namespace VoltRpc.Communication;

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

    /// <summary>
    ///     All the added services
    /// </summary>
    internal readonly List<HostService> Services = new();

    /// <summary>
    ///     Version this host wants to be
    /// </summary>
    internal Versioning.VersionInfo version;

    private int maxConnectionsCount = 16;

    /// <summary>
    ///     Creates a new <see cref="Host" /> instance
    /// </summary>
    /// <param name="logger">The <see cref="ILogger" /> to use</param>
    /// <param name="bufferSize">The initial size of the buffers</param>
    /// <exception cref="ArgumentOutOfRangeException">Will throw if the buffer size is less then 16</exception>
    protected Host(ILogger? logger = null, int bufferSize = DefaultBufferSize)
    {
        if (bufferSize < 16)
            throw new ArgumentOutOfRangeException(nameof(bufferSize),
                "The buffer needs to be larger then 15 bytes!");

        Logger = logger ?? new NullLogger();

        TypeReaderWriterManager = new TypeReaderWriterManager();
        invokeLock = new object();

        BufferSize = bufferSize;
        version = Versioning.Version;
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

    /// <summary>
    ///     Is the <see cref="Host" /> running?
    /// </summary>
    public bool IsRunning { get; protected set; }

    /// <summary>
    ///     Hides the stacktrace from the client when an <see cref="Exception" /> is thrown
    /// </summary>
    public bool HideStacktrace { get; set; }

    /// <summary>
    ///     User set protocol info
    /// </summary>
    private ProtocolInfo? protocolInfo;

    /// <summary>
    ///     Starts listening for incoming requests
    /// </summary>
    public virtual async Task StartListeningAsync()
    {
        await Task.Run(StartListening);
    }

    /// <summary>
    ///     Starts listening for incoming requests
    /// </summary>
    public abstract void StartListening();

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
        CheckDispose();

        AddService(typeof(T), service);
    }

    /// <summary>
    ///     Adds a service to this <see cref="Host" />
    /// </summary>
    /// <param name="serviceType">The <see cref="Type" /> of a service</param>
    /// <param name="serviceObject">The actual service <see cref="object" /> itself</param>
    /// <exception cref="ArgumentException">Thrown if the service has already been added</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if serviceType is not an interface</exception>
#if NET6_0_OR_GREATER
    public void AddService([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type serviceType,
#else
        public void AddService(Type serviceType,
#endif
        object serviceObject)
    {
        CheckDispose();

        if (!serviceType.IsInterface)
            throw new ArgumentOutOfRangeException(nameof(serviceType), "Service Type is not an interface!");

        if (Services.Exists(x => x.InterfaceObject == serviceObject
                                 || x.InterfaceName == serviceType.FullName))
            throw new ArgumentException("The service already exists!", nameof(serviceType));

        Services.Add(new HostService
        {
            InterfaceName = serviceType.FullName,
            InterfaceObject = serviceObject,
            ServiceMethods = GetAllServiceMethods()
        });

        //TODO: Go back to what we were using when this shit is fixed
        //Recommend hack for fixing the trimming warning
        //https://github.com/dotnet/linker/issues/2487
#if NET6_0
        [UnconditionalSuppressMessage("Trimming", "IL2077", Justification = "The type parameter of the parent method has the right annotation")]
#endif
        ServiceMethod[] GetAllServiceMethods() => ServiceHelper.GetAllServiceMethods(serviceType);
    }

    /// <summary>
    ///     Sets what protocol version to use.
    ///     <para>Set value to null to reset back to none.</para>
    /// </summary>
    /// <param name="value">Value can be any object you want, as long as the <see cref="TypeReaderWriterManager"/> has a <see cref="TypeReadWriter{T}"/> for it.</param>
    /// <exception cref="AlreadyConnectedException">Thrown if the <see cref="Client"/> is already connected to a <see cref="Host"/></exception>
    /// <exception cref="NoTypeReaderWriterException">Thrown if the <see cref="TypeReaderWriterManager"/> doesn't have a <see cref="TypeReadWriter{T}"/> for the value <see cref="Type"/>.</exception>
    public void SetProtocolVersion(object? value)
    {
        CheckDispose();

        if (IsRunning)
            throw new AlreadyRunningException();
        
        //If type is null, then reset protocol info
        if (value == null)
        {
            protocolInfo = null;
            return;
        }

        Type valueType = value.GetType();
        ITypeReadWriter? typeReadWriter = TypeReaderWriterManager.GetType(valueType);
        if (typeReadWriter == null)
            throw new NoTypeReaderWriterException($"The type reader/writer for {valueType} doesn't exist! You need to add it to {nameof(TypeReaderWriterManager)} first before calling {nameof(SetProtocolVersion)}.");

        protocolInfo = new ProtocolInfo(value, valueType);
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
        CheckDispose();

        if (readStream == null)
            throw new ArgumentNullException(nameof(readStream));

        if (writeStream == null)
            throw new ArgumentNullException(nameof(writeStream));

        if (!readStream.CanRead)
            throw new ArgumentOutOfRangeException(nameof(readStream), "The read stream cannot be read to!");

        if (!writeStream.CanWrite)
            throw new ArgumentOutOfRangeException(nameof(writeStream), "The write stream cannot be wrote to!");

        BufferedReader reader = new(readStream, BufferSize);
        BufferedWriter writer = new(writeStream, BufferSize);

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
        CheckDispose();

        if (reader == null)
            throw new ArgumentNullException(nameof(reader));
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));
        
        bool doContinue = ProcessSyncCheck(reader, writer);
        
        //Now for the message loop
        while (doContinue)
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
                Logger.Warn("Client disconnected, most likely from a timeout.");
            }
        }

        reader.Dispose();
        writer.Dispose();
    }

    private void ProcessInvokeMethod(BufferedReader reader, BufferedWriter writer)
    {
        lock (invokeLock)
        {
            //First, read the method
            string methodName = reader.ReadString();

            object? obj = null;
            ServiceMethod? method = null;
            foreach (HostService service in Services)
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
                WriteError(writer, MessageResponse.MethodNotFound);
                Logger.Warn("Client sent an invalid method request.");
                return;
            }

            //Now we read the parameters
            int paramsCount = method.Parameters.Length;
            object?[] parameters = new object[paramsCount];
            for (int i = 0; i < paramsCount; i++)
            {
                ServiceMethodParameter parameter = method.Parameters[i];

                //If it is a out, then we just set it to null and don't read (as nothing is sent for outs)
                if (parameter.IsOut)
                {
                    parameters[i] = null;
                    continue;
                }

                //Get the type reader
                ITypeReadWriter typeRead = TypeReaderWriterManager.GetType(parameter.TypeInfo.TypeName);
                if (typeRead == null)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFailMissing);
                    Logger.Error(
                        $"The client sent a method with a parameter type of '{parameter.TypeInfo.TypeName}' of which I don't have a type reader for some reason!");
                    return;
                }

                try
                {
                    parameters[i] = TypeReaderWriterManager.Read(reader, typeRead, parameter.TypeInfo);
                }
                catch (Exception ex)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFail, ex.Message,
                        ex.InnerException?.StackTrace);
                    Logger.Error($"Error reading parameter! {ex}");
                    return;
                }
            }

            //Invoke the method
            object? methodReturn;
            try
            {
                methodReturn = method.MethodInfo.Invoke(obj, parameters);
            }
            catch (Exception ex)
            {
                WriteError(writer, MessageResponse.MethodExecuteFailException, ex.Message,
                    ex.InnerException?.StackTrace);
                Logger.Error($"Method invoke failed! {ex}");
                return;
            }

            writer.WriteByte((byte) MessageResponse.MethodExecutedSuccessful);

            //If the method doesn't return void, write it back
            if (!method.IsReturnVoid)
            {
                ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(method.ReturnType.TypeName);
                if (typeWriter == null)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFailMissing);
                    Logger.Error(
                        $"The client sent a method with a return type of '{method.ReturnType.TypeName}' of which I don't have a type reader for some reason!");
                    return;
                }

                try
                {
                    TypeReaderWriterManager.Write(writer, typeWriter, method.ReturnType, methodReturn);
                }
                catch (Exception ex)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFail, ex.Message,
                        ex.InnerException?.StackTrace);
                    Logger.Error($"Parsing return type of '{method.ReturnType.TypeName}' failed for some reason! {ex}");
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

                    ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(parameter.TypeInfo.TypeName);
                    if (typeWriter == null)
                    {
                        WriteError(writer, MessageResponse.TypeReadWriterFailMissing);
                        Logger.Error(
                            $"The client sent a method with a parameter ref/out type of '{parameter.TypeInfo.TypeName}' of which I don't have a type reader for some reason!");
                        return;
                    }

                    try
                    {
                        TypeReaderWriterManager.Write(writer, typeWriter, parameter.TypeInfo, parameters[i]);
                    }
                    catch (Exception ex)
                    {
                        WriteError(writer, MessageResponse.TypeReadWriterFail, ex.Message,
                            ex.InnerException?.StackTrace);
                        Logger.Error(
                            $"Parsing return type of '{parameter.TypeInfo.TypeName}' failed for some reason! {ex}");
                        return;
                    }
                }

            writer.Flush();
        }
    }

    /// <summary>
    ///     Process a sync check message
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="writer"></param>
    /// <returns>Returns true if sync message was good</returns>
    private bool ProcessSyncCheck(BufferedReader reader, BufferedWriter writer)
    {
        //Check sync info first.
        try
        {
            byte major = reader.ReadByte();
            byte minor = reader.ReadByte();
            byte patch = reader.ReadByte();

            //Version info doesn't match to ours
            if (major != version.Major || minor != version.Minor || patch != version.Patch)
            {
                WriteError(writer, MessageResponse.SyncVersionMissMatch);
                Logger.Warn("Refusing client, version info doesn't match!");
                return false;
            }
            
            //Check user protocol
            bool hasProtocol = reader.ReadBool();
            
            //Make sure we both agree if a protocol exists or not
            if (hasProtocol != protocolInfo.HasValue)
            {
                WriteError(writer, MessageResponse.SyncProtocolExistenceMissMatch);
                Logger.Warn("Refusing client, protocol status was miss-matched to ours!");
                return false;
            }
            
            //We have protocol info, now check it
            if(protocolInfo != null)
            {
                ProtocolInfo protocol = protocolInfo.Value;
                
                //Read protocol value type
                string type = reader.ReadString();
                if (protocol.TypeInfo.TypeName != type)
                {
                    WriteError(writer, MessageResponse.SyncProtocolTypeMissMatch);
                    Logger.Warn("Refusing client, protocol value type was miss-matched to ours!");
                    return false;
                }

                ITypeReadWriter? typeReadWriter = TypeReaderWriterManager.GetType(protocol.TypeInfo.TypeName);
                if (typeReadWriter == null)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFailMissing);
                    Logger.Error($"The client sent a protocol type of {type}, which matched mine, but I don't have the type reader/writer for!");
                    return false;
                }

                try
                {
                    object result = TypeReaderWriterManager.Read(reader, typeReadWriter, protocol.TypeInfo);
                    object protocolValue = protocol.Value;
                    
                    //Compare result to our protocol value
                    if (!protocolValue.Equals(result))
                    {
                        WriteError(writer, MessageResponse.SyncProtocolValueMissMatch);
                        Logger.Warn($"Refusing client, protocol values was miss-matched to ours!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    WriteError(writer, MessageResponse.TypeReadWriterFail, ex.Message, ex.StackTrace);
                    Logger.Error($"Error reading protocol value! {ex}");
                    return false;
                }
            }
            
            //Check services now
            ushort serviceCount = reader.ReadUShort();
            if (Services.Count != serviceCount)
            {
                WriteError(writer, MessageResponse.SyncServiceMissMatch, "Services count do not match!");
                Logger.Warn("Refusing client, client's services count miss-matched from mine!");
                return false;
            }
            
            for (int i = 0; i < serviceCount; i++)
            {
                //Read service name
                string serviceName = reader.ReadString();
                HostService? service = Services.SingleOrDefault(x => x.InterfaceName == serviceName);
                if (service == null)
                {
                    WriteError(writer, MessageResponse.SyncServiceMissMatch, $"Service {serviceName} doesn't exist on host!");
                    Logger.Warn($"Refusing client, client had service {serviceName} that doesn't exist!");
                    return false;
                }

                //Check service methods
                ushort methodCount = reader.ReadUShort();
                
                //ServiceMethods can be null
                int methodCountTrue = 0;
                if (service.Value.ServiceMethods != null)
                    methodCountTrue = service.Value.ServiceMethods.Length;
                
                if (methodCount != methodCountTrue)
                {
                    WriteError(writer, MessageResponse.SyncServiceMissMatch, $"Service {serviceName} method count do not match!");
                    Logger.Warn($"Refusing client, client had {serviceName} method count that miss-matched from mine!");
                    return false;
                }

                for (int j = 0; j < methodCount; j++)
                {
                    string methodName = reader.ReadString();
                    ServiceMethod? method = service.Value.ServiceMethods!.SingleOrDefault(x => x.MethodName == methodName);
                    if (method == null)
                    {
                        WriteError(writer, MessageResponse.SyncServiceMissMatch, $"The {methodName} doesn't exist as apart of {serviceName}!");
                        Logger.Warn($"Refusing client, client had {methodName} as listed apart of {serviceName}!");
                        return false;
                    }
                    
                    //Check method parameters
                    ushort parameterCount = reader.ReadUShort();
                    int parameterCountTrue = 0;
                    if (method.Parameters != null)
                        parameterCountTrue = method.Parameters.Length;
                    
                    if (parameterCount != parameterCountTrue)
                    {
                        WriteError(writer, MessageResponse.SyncServiceMissMatch, $"The {methodName} parameter count is miss-matched!");
                        Logger.Warn($"Refusing client, client had {methodName} parameter count that miss-matched from mine!");
                        return false;
                    }

                    //Check parameter
                    for (int k = 0; k < parameterCount; k++)
                    {
                        string parameterName = reader.ReadString();
                        
                        //Unlike services or methods, the order of this DOES matter
                        if (method.Parameters![k].TypeInfo.TypeName != parameterName)
                        {
                            WriteError(writer, MessageResponse.SyncServiceMissMatch, $"The {methodName}'s parameter {parameterName}");
                        }
                    }
                }
            }
            
            //We all good
            Logger.Debug("Accepted client, client's sync info matches.");
            writer.WriteByte((byte)MessageResponse.SyncRighto);
            writer.Flush();
            return true;
        }
        catch (IOException)
        {
            //Timeout most likely
            Logger.Warn("Client never sent version info in time!");
            return false;
        }
    }

    private void WriteError(BufferedWriter writer, MessageResponse message, string? error = null,
        string? stackTrace = null)
    {
        writer.Reset();
        writer.WriteByte((byte) message);
        switch (message)
        {
            case MessageResponse.MethodNotFound:
            case MessageResponse.MethodExecutedSuccessful:
            case MessageResponse.SyncProtocolExistenceMissMatch:
            case MessageResponse.SyncProtocolTypeMissMatch:
            case MessageResponse.SyncProtocolValueMissMatch:
            case MessageResponse.TypeReadWriterFailMissing:
                break;
            case MessageResponse.SyncVersionMissMatch:
                //Write what version we are excepting
                writer.WriteByte(version.Major);
                writer.WriteByte(version.Minor);
                writer.WriteByte(version.Patch);
                break;
            case MessageResponse.SyncServiceMissMatch:
                writer.WriteString(error);
                break;
            case MessageResponse.TypeReadWriterFail:
            case MessageResponse.MethodExecuteFailException:
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
    ///     Checks if the object has been disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    protected void CheckDispose()
    {
        if (HasDisposed)
            throw new ObjectDisposedException(nameof(Host));
    }

    /// <summary>
    ///     Has this object been disposed
    /// </summary>
    public bool HasDisposed { get; private set; }

    /// <summary>
    ///     Destroys the <see cref="Host" /> instance
    /// </summary>
    public virtual void Dispose()
    {
        CheckDispose();
        HasDisposed = true;
        IsRunning = false;
        GC.SuppressFinalize(this);
    }

    #endregion
}