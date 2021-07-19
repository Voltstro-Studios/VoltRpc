using System;
using System.Collections.Generic;
using System.IO;
using VoltRpc.IO;
using VoltRpc.Proxy;
using VoltRpc.Types;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Client"/> sends messages to a <see cref="Host"/>
    /// </summary>
    public abstract class Client : IDisposable
    {
        private BufferedReader binReader;
        private BufferedWriter binWriter;

        private readonly TypeReaderWriterManager typeReaderWriterManager;
        private readonly List<ServiceMethod> methods;

        /// <summary>
        ///     Creates a new <see cref="Client"/> instance
        /// </summary>
        protected Client()
        {
            typeReaderWriterManager = new TypeReaderWriterManager();
            methods = new List<ServiceMethod>();
        }

        /// <summary>
        ///     Internal usage for if the client is connected
        /// </summary>
        protected bool IsConnectedInternal;

        /// <summary>
        ///     Is the <see cref="Client"/> connected
        /// </summary>
        public bool IsConnected => IsConnectedInternal;

        /// <summary>
        ///     Connects the <see cref="Client"/> to a host
        /// </summary>
        public abstract void Connect();

        /// <summary>
        ///     Tells the <see cref="Client"/> what interfaces you might be using
        /// </summary>
        /// <typeparam name="T">The same interface that you are using on the server</typeparam>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if T is not an interface</exception>
        public void AddService<T>()
            where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentOutOfRangeException(nameof(T), "T is not an interface!");
            
            methods.AddRange(ServiceHelper.GetAllServiceMethods<T>());
        }

        /// <summary>
        ///     Initialize streams
        /// </summary>
        /// <param name="stream">The main stream to communicate on</param>
        protected void Initialize(Stream stream)
        {
            binReader = new BufferedReader(stream);
            binWriter = new BufferedWriter(stream);
        }

        /// <summary>
        ///     Invokes a method on the server
        /// </summary>
        /// <param name="methodName">The full method name</param>
        /// <param name="parameters">All parameters to be passed to the method</param>
        /// <exception cref="MissingMethodException">
        ///     Thrown if the method name doesn't exist on either the client or server</exception>
        /// <exception cref="NoTypeReaderWriterException">
        ///     Thrown if the return type or parameter types doesn't have a <see cref="ITypeReadWriter"/>
        /// </exception>
        public object InvokeMethod(string methodName, params object[] parameters)
        {
            //Get the method
            ServiceMethod method = null;
            foreach (ServiceMethod serviceMethod in methods)
            {
                if (serviceMethod.MethodName != methodName) 
                    continue;
                method = serviceMethod;
                break;
            }

            if (method == null)
                throw new MissingMemberException($"The interface that {methodName} is from needs to be added first with AddService!");

            //Write the method name first
            binWriter.WriteByte((byte) MessageType.InvokeMethod);
            binWriter.WriteString(method.MethodName);

            //Now we need to write our parameters
            WriteParams(method, parameters);

            binWriter.Flush();

            //Get response
            MessageResponse response = (MessageResponse) binReader.ReadByte();
            switch (response)
            {
                case MessageResponse.NoMethodFound:
                    throw new MissingMethodException("The method does not exist on the server!");
                case MessageResponse.ExecuteFailNoTypeReader:
                    throw new NoTypeReaderWriterException();
                case MessageResponse.ExecuteTypeReadWriteFail:
                {
                    string reason = binReader.ReadString();
                    throw new Exception($"Type reader failed to read: {reason}");
                }
                case MessageResponse.ExecuteInvokeFailException:
                {
                    string reason = binReader.ReadString();
                    throw new Exception($"The method failed for some reason: {reason}");
                }
            }
            
            //If we are a void, then we need to read the response
            if (!method.IsReturnVoid)
            {
                //Get the type reader
                ITypeReadWriter typeReader = typeReaderWriterManager.GetType(method.ReturnTypeName);
                if(typeReader == null)
                    throw new NoTypeReaderWriterException();

                return typeReader.Read(binReader);
            }

            return null;
        }

        /// <summary>
        ///     Writes the parameters
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <exception cref="NoTypeReaderWriterException"></exception>
        private void WriteParams(ServiceMethod method, IReadOnlyList<object> parameters)
        {
            binWriter.WriteInt(parameters.Count);

            for (int i = 0; i < parameters.Count; i++)
            {
                string parameterTypeName = method.ParametersTypeNames[i];
                object parameter = parameters[i];
                ITypeReadWriter writer = typeReaderWriterManager.GetType(parameterTypeName);
                if (writer == null)
                    throw new NoTypeReaderWriterException();

                binWriter.WriteString(parameterTypeName);
                writer.Write(binWriter, parameter);
            }
        }

        /// <summary>
        ///     Destroys the <see cref="Client"/> instance
        /// </summary>
        public virtual void Dispose()
        {
            if (IsConnectedInternal)
            {
                binWriter.WriteByte((byte)MessageType.Shutdown);
                binWriter.Flush();
            }
            
            binReader.Dispose();
            binWriter.Dispose();
        }
    }
}