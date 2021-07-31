using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VoltRpc.IO;
using VoltRpc.Services;
using VoltRpc.Types;

namespace VoltRpc.Communication
{
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

        private readonly Dictionary<string, ServiceMethod[]> services;

        /// <summary>
        ///     Internal usage for if the client is connected
        /// </summary>
        protected bool IsConnectedInternal;

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
            services = new Dictionary<string, ServiceMethod[]>();
            this.bufferSize = bufferSize;
        }

        /// <summary>
        ///     The <see cref="Types.TypeReaderWriterManager" /> for <see cref="Client" />
        /// </summary>
        public TypeReaderWriterManager TypeReaderWriterManager { get; }

        /// <summary>
        ///     Is the <see cref="Client" /> connected
        /// </summary>
        public bool IsConnected => IsConnectedInternal;

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
        public void AddService<T>()
            where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
                throw new ArgumentOutOfRangeException(nameof(T), "T is not an interface!");

            if (interfaceType.FullName == null)
                throw new NullReferenceException("T's Type.FullName is null!");

            if (services.ContainsKey(interfaceType.FullName))
                throw new ArgumentOutOfRangeException(nameof(T), "T has already been added as a service!");

            services.Add(interfaceType.FullName, ServiceHelper.GetAllServiceMethods<T>());
        }

        /// <summary>
        ///     Initialize streams
        /// </summary>
        /// <param name="readStream">The read stream</param>
        /// <param name="writeStream">The write stream</param>
        /// <exception cref="ArgumentNullException">Thrown if either provide stream is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if we can't read or write to the respected stream</exception>
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

            reader = new BufferedReader(readStream, bufferSize);
            writer = new BufferedWriter(writeStream, bufferSize);
        }

        /// <summary>
        ///     Invokes a method on the server
        /// </summary>
        /// <param name="methodName">The full method name</param>
        /// <param name="parameters">All parameters to be passed to the method</param>
        /// <exception cref="MissingMethodException">
        ///     Thrown if the method name doesn't exist on either the client or server
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
            //Get the method
            ServiceMethod method = null;
            foreach (KeyValuePair<string, ServiceMethod[]> service in services)
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
                ITypeReadWriter typeReader = TypeReaderWriterManager.GetType(method.ReturnTypeName);
                if (typeReader == null)
                    throw new NoTypeReaderWriterException();

                objectReturn[0] = typeReader.Read(reader);
                currentObjectReturnIndex++;
            }

            //Read our ref and out parameters
            if (method.ContainsRefOrOutParameters)
                foreach (ServiceMethodParameter parameter in method.Parameters)
                {
                    if (!parameter.IsRef && !parameter.IsOut)
                        continue;

                    //Get the type reader
                    ITypeReadWriter typeReader = TypeReaderWriterManager.GetType(parameter.ParameterTypeName);
                    if (typeReader == null)
                        throw new NoTypeReaderWriterException();

                    objectReturn[currentObjectReturnIndex] = typeReader.Read(reader);
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
                string parameterTypeName = method.Parameters[i].ParameterTypeName;
                object parameter = parameters[i];
                ITypeReadWriter typeWriter = TypeReaderWriterManager.GetType(parameterTypeName);
                if (typeWriter == null)
                    throw new NoTypeReaderWriterException();

                typeWriter.Write(writer, parameter);
            }
        }

        #region Destroy

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
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
            if (IsConnectedInternal)
            {
                writer.WriteByte((byte) MessageType.Shutdown);
                writer.Flush();
            }

            reader.Dispose();
            writer.Dispose();
        }

        #endregion
    }
}