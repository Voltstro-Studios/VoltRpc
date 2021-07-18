﻿using System;
using System.Collections.Generic;
using System.IO;
using VoltRpc.Proxy;
using VoltRpc.Types;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Client"/> sends messages to a <see cref="Host"/>
    /// </summary>
    public abstract class Client : IDisposable
    {
        private BinaryReader binReader;
        private BinaryWriter binWriter;

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
        ///     Tells the <see cref="Client"/> that you are going to use <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">The same interface that you are using on the server</typeparam>
        public void AddService<T>()
        {
            methods.AddRange(ServiceHelper.GetAllServiceMethods<T>());
        }

        /// <summary>
        ///     Sends the init message to the server
        /// </summary>
        /// <param name="stream"></param>
        protected void Initialize(Stream stream)
        {
            binReader = new BinaryReader(stream);
            binWriter = new BinaryWriter(stream);
        }

        /// <summary>
        ///     Invokes a method on the server
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="NoTypeReaderWriterException"></exception>
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
                throw new NullReferenceException($"The interface that {methodName} is from needs to be added first with AddService!");

            //Write the method name first
            binWriter.Write((int) MessageType.InvokeMethod);
            binWriter.Write(method.MethodName);

            //Now we need to write our parameters
            WriteParams(method, parameters);

            binWriter.Flush();

            //Get response
            MessageResponse response = (MessageResponse) binReader.ReadInt32();
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
            
            //If we are not a void, then we need to read the response
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
            binWriter.Write(parameters.Count);

            for (int i = 0; i < parameters.Count; i++)
            {
                object parameter = parameters[i];
                ITypeReadWriter writer = typeReaderWriterManager.GetType(parameter);
                if (writer == null)
                    throw new NoTypeReaderWriterException();

                binWriter.Write(method.ParametersTypeNames[i]);
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
                binWriter.Write((int)MessageType.Shutdown);
                binWriter.Flush();
            }
            
            binReader.Dispose();
            binWriter.Dispose();
        }
    }
}