using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using VoltRpc.IO;
using VoltRpc.Services;
using VoltRpc.Types;

namespace VoltRpc.Communication;

/// <summary>
///     The <see cref="Client" /> sends messages to a <see cref="Host" />
/// </summary>
public abstract class Client : IDisposable
{
    /// <summary>
    ///     The default size of the buffers
    /// </summary>
    public const int DefaultBufferSize = 8000;

    private readonly int bufferSize;

    /// <summary>
    ///     All added services
    /// </summary>
    internal readonly Dictionary<string, ServiceMethod[]> Services;

    private BufferedReader reader;
    private BufferedWriter writer;

    /// <summary>
    ///     Creates a new <see cref="Client" /> instance
    /// </summary>
    /// <param name="bufferSize">The initial size of the buffers</param>
    /// <exception cref="ArgumentOutOfRangeException">Will throw if the buffer size is less then 16</exception>
    protected Client(int bufferSize = DefaultBufferSize)
    {
        if (bufferSize < 16)
            throw new ArgumentOutOfRangeException(nameof(bufferSize),
                "The buffer needs to be larger then 15 bytes!");

        TypeReaderWriterManager = new TypeReaderWriterManager();
        Services = new Dictionary<string, ServiceMethod[]>();
        this.bufferSize = bufferSize;
    }

    /// <summary>
    ///     The <see cref="Types.TypeReaderWriterManager" /> for <see cref="Client" />
    /// </summary>
    public TypeReaderWriterManager TypeReaderWriterManager { get; }

    /// <summary>
    ///     Is the <see cref="Client" /> connected
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    ///     Connects the <see cref="Client" /> to a host
    /// </summary>
    public abstract void Connect();

    /// <summary>
    ///     Tells the <see cref="Client" /> what interfaces you might be using
    /// </summary>
    /// <typeparam name="T">The same interface that you are using on the server</typeparam>
    /// <exception cref="NullReferenceException">Thrown if T's <see cref="Type.FullName" /> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if T is not an interface, or has already been added as a service.</exception>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Use AddService(Type interfaceType) instead.")]
#endif
    public void AddService<T>()
        where T : class
    {
        CheckDispose();

        AddService(typeof(T));
    }

    /// <summary>
    ///     Tells the <see cref="Client" /> what interfaces you might be using
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if interfaceType's <see cref="Type.FullName" /> is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if interfaceType is not an interface, or has already been added as
    ///     a service.
    /// </exception>
#if NET6_0_OR_GREATER
    public void AddService(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        Type interfaceType)
#else
        public void AddService(Type interfaceType)
#endif
    {
        CheckDispose();

        if (!interfaceType.IsInterface)
            throw new ArgumentOutOfRangeException(nameof(interfaceType),
                "Provided interface type is not an interface!");

        if (interfaceType.FullName == null)
            throw new NullReferenceException("interfaceType.FullName is null!");

        if (Services.ContainsKey(interfaceType.FullName))
            throw new ArgumentOutOfRangeException(nameof(interfaceType),
                "interfaceType has already been added as a service!");

        Services.Add(interfaceType.FullName, ServiceHelper.GetAllServiceMethods(interfaceType));
    }

    /// <summary>
    ///     Initialize streams
    /// </summary>
    /// <param name="readStream">The <see cref="Stream" /> to read from</param>
    /// <param name="writeStream">The <see cref="Stream" /> to write to</param>
    /// <exception cref="ArgumentNullException">Thrown if either provided stream is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if we can't read or write to the respected streams</exception>
    protected void Initialize(Stream readStream, Stream writeStream)
    {
        if (readStream == null)
            throw new ArgumentNullException(nameof(readStream));

        if (writeStream == null)
            throw new ArgumentNullException(nameof(writeStream));

        if (!readStream.CanRead)
            throw new ArgumentOutOfRangeException(nameof(readStream), "The read stream cannot be read to!");

        if (!writeStream.CanWrite)
            throw new ArgumentOutOfRangeException(nameof(writeStream), "The write stream cannot be wrote to!");

        Initialize(new BufferedReader(readStream, bufferSize), new BufferedWriter(writeStream, bufferSize));
    }

    /// <summary>
    ///     Initialize streams
    /// </summary>
    /// <param name="bufferedRead">The <see cref="BufferedReader" /> that will be read from</param>
    /// <param name="bufferedWrite">The <see cref="BufferedWriter" /> that will be written to</param>
    /// <exception cref="ArgumentNullException">Thrown if either provided buffers is null</exception>
    protected void Initialize(BufferedReader bufferedRead, BufferedWriter bufferedWrite)
    {
        CheckDispose();

        reader = bufferedRead ?? throw new ArgumentNullException(nameof(bufferedRead));
        writer = bufferedWrite ?? throw new ArgumentNullException(nameof(bufferedWrite));
        IsConnected = true;
    }

    /// <summary>
    ///     Invokes a method on the server
    /// </summary>
    /// <param name="methodName">The full method name</param>
    /// <param name="parameters">All parameters to be passed to the method</param>
    /// <exception cref="NotConnectedException">Thrown if the client is not connected to a host</exception>
    /// <exception cref="MissingMethodException">
    ///     Thrown if the method name doesn't exist on either the client or host
    /// </exception>
    /// <exception cref="NoTypeReaderWriterException">
    ///     Thrown if the return type or parameter types doesn't have a <see cref="ITypeReadWriter" />
    /// </exception>
    /// <exception cref="TypeReaderWriterException">
    ///     Thrown if the type reader/writer fails on the host
    /// </exception>
    /// <exception cref="MethodInvokeFailedException">
    ///     Thrown if an <see cref="Exception" /> occurs while invoking a method on the host
    /// </exception>
    public object[] InvokeMethod(string methodName, params object[] parameters)
    {
        CheckDispose();

        if (!IsConnected)
            throw new NotConnectedException("The client is not connected!");

        //Get the method
        ServiceMethod method = null;
        foreach (KeyValuePair<string, ServiceMethod[]> service in Services)
        {
            if (method != null)
                break;

            foreach (ServiceMethod serviceMethod in service.Value)
            {
                if (serviceMethod.MethodName != methodName)
                    continue;
                method = serviceMethod;
                break;
            }
        }

        if (method == null)
            throw new MissingMethodException(
                $"The interface that {methodName} is from needs to be added first with AddService!");

        //Write the method name first
        writer.WriteByte((byte) MessageType.InvokeMethod);
        writer.WriteString(method.MethodName);

        //Now we need to write our parameters
        WriteParams(method, parameters);

        writer.Flush();

        //Get response
        MessageResponse response = (MessageResponse) reader.ReadByte();
        switch (response)
        {
            case MessageResponse.NoMethodFound:
                throw new MissingMethodException("The method does not exist on the server!");
            case MessageResponse.ExecuteFailNoTypeReader:
                throw new NoTypeReaderWriterException();
            case MessageResponse.ExecuteTypeReadWriteFail:
            {
                string reason = reader.ReadString();
                string stackTrace = reader.ReadString();
                throw new TypeReaderWriterException("An error occured in type reader/writer on the host!",
                    new StackTrace().ToString(), reason, stackTrace);
            }
            case MessageResponse.ExecuteInvokeFailException:
            {
                string reason = reader.ReadString();
                string stackTrace = reader.ReadString();
                throw new MethodInvokeFailedException("Invoking the method threw an exception on the host!",
                    new StackTrace().ToString(), reason, stackTrace);
            }
        }

        //We can fuck off from here
        if (method.IsReturnVoid && !method.ContainsRefOrOutParameters)
            return null;

        //Create out array to return out of this method with
        int objectReturnSize = method.RefOrOutParameterCount;
        if (!method.IsReturnVoid)
            objectReturnSize++;

        object[] objectReturn = new object[objectReturnSize];
        int currentObjectReturnIndex = 0;

        //If we are a void, then we need to read the response
        if (!method.IsReturnVoid)
        {
            //Get the type reader
            ITypeReadWriter typeReader = TypeReaderWriterManager.GetType(method.ReturnType.TypeName);
            if (typeReader == null)
                throw new NoTypeReaderWriterException();

            objectReturn[0] = TypeReaderWriterManager.Read(reader, typeReader, method.ReturnType);
            currentObjectReturnIndex++;
        }

        //Read our ref and out parameters
        if (method.ContainsRefOrOutParameters)
            foreach (ServiceMethodParameter parameter in method.Parameters)
            {
                if (!parameter.IsRef && !parameter.IsOut)
                    continue;

                //Get the type reader
                ITypeReadWriter typeReader = TypeReaderWriterManager.GetType(parameter.TypeInfo.TypeName);
                if (typeReader == null)
                    throw new NoTypeReaderWriterException();

                objectReturn[currentObjectReturnIndex] =
                    TypeReaderWriterManager.Read(reader, typeReader, parameter.TypeInfo);
                currentObjectReturnIndex++;
            }

        return objectReturn;
    }

    /// <summary>
    ///     Writes the parameters
    /// </summary>
    /// <param name="method"></param>
    /// <param name="parameters"></param>
    /// <exception cref="NoTypeReaderWriterException"></exception>
    private void WriteParams(ServiceMethod method, IReadOnlyList<object> parameters)
    {
        for (int i = 0; i < parameters.Count; i++)
        {
            string parameterTypeName = method.Parameters[i].TypeInfo.TypeName;
            object parameter = parameters[i];
            ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(parameterTypeName);
            if (typeWriter == null)
                throw new NoTypeReaderWriterException();

            TypeReaderWriterManager.Write(writer, typeWriter, method.Parameters[i].TypeInfo, parameter);
        }
    }

    #region Destroy

    /// <summary>
    ///     Has this object been disposed
    /// </summary>
    public bool HasDisposed { get; private set; }

    /// <summary>
    ///     Checks if the object has been disposed
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    protected void CheckDispose()
    {
        if (HasDisposed)
            throw new ObjectDisposedException(nameof(Client));
    }

    /// <summary>
    ///     Deconstructor for <see cref="Client" />.
    ///     <para>
    ///         Tells the server that we have disconnected and releases resources if it hasn't been done by
    ///         <see cref="Dispose" /> already.
    ///     </para>
    /// </summary>
    ~Client()
    {
        ReleaseResources();
    }

    /// <summary>
    ///     Destroys the <see cref="Client" /> instance
    /// </summary>
    public virtual void Dispose()
    {
        CheckDispose();
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseResources()
    {
        if (IsConnected)
        {
            writer?.WriteByte((byte) MessageType.Shutdown);
            writer?.Flush();
        }

        reader?.Dispose();
        writer?.Dispose();
        IsConnected = false;
        HasDisposed = true;
    }

    #endregion
}