using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        /// <summary>
        ///     Creates a new <see cref="Host"/> instance
        /// </summary>
        protected Host()
        {
            readerWriterManager = new TypeReaderWriterManager();
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
        public void AddService<T>(T service) 
            where T : class
        {
            if (methods.ContainsKey(service))
                throw new ArgumentException("The service already exists!", nameof(service));

            ServiceMethod[] serviceMethods = ServiceHelper.GetAllServiceMethods<T>();
            methods.Add(service, serviceMethods);
        }

        /// <summary>
        ///     Processes a request from a client
        /// </summary>
        /// <param name="readStream"></param>
        /// <param name="writeStream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        protected void ProcessRequest(Stream readStream, Stream writeStream)
        {
            if (readStream == null)
                throw new ArgumentNullException(nameof(readStream));
            
            if (writeStream == null)
                throw new ArgumentNullException(nameof(writeStream));

            if (!readStream.CanRead)
                throw new ArgumentOutOfRangeException(nameof(readStream), "The read stream cannot be read to!");
            
            if (!writeStream.CanRead)
                throw new ArgumentOutOfRangeException(nameof(writeStream), "The write stream cannot be wrote to!");

            BinaryReader binReader = new BinaryReader(readStream);
            BinaryWriter binWriter = new BinaryWriter(writeStream);
            
            bool doContinue = true;
            do
            {
                MessageType messageType = (MessageType) binReader.ReadInt32();
                switch (messageType)
                {
                    case MessageType.InvokeMethod:
                        ProcessInvokeMethod(binReader, binWriter);
                        break;
                    case MessageType.Shutdown:
                        doContinue = false;
                        break;
                }


            } while (doContinue);
            
            binReader.Dispose();
            binWriter.Dispose();
        }

        private void ProcessInvokeMethod(BinaryReader reader, BinaryWriter writer)
        {
            //First, read the method
            string methodName = reader.ReadString();

            object obj = null;
            MethodInfo methodInfo = null;
            foreach (KeyValuePair<object, ServiceMethod[]> method in methods)
            {
                if(methodInfo != null)
                    break;
                
                foreach (ServiceMethod serviceMethod in method.Value)
                {
                    if (serviceMethod.MethodName == methodName)
                    {
                        obj = method.Key;
                        methodInfo = serviceMethod.MethodInfo;
                    }
                }
            }

            //No method was found
            if (methodInfo == null)
            {
                writer.Write((int)MessageResponse.NoMethodFound);
                writer.Flush();
                return;
            }
            
            //Now we read the parameters
            int paramsCount = reader.ReadInt32();
            object[] parameters = new object[paramsCount];
            for (int i = 0; i < paramsCount; i++)
            {
                //Read the type
                string type = reader.ReadString();
                ITypeReadWriter typeRead = readerWriterManager.GetType(type);
                if (typeRead == null)
                {
                    writer.Write((int)MessageResponse.ExecuteFailNoTypeReader);
                    writer.Flush();
                    return;
                }

                try
                {
                    parameters[i] = typeRead.Read(reader);
                }
                catch (Exception ex)
                {
                    writer.Write((int)MessageResponse.ExecuteTypeReaderFail);
                    writer.Write(ex.Message);
                    writer.Flush();
                    return;
                }
            }

            //Invoke the method
            try
            {
                methodInfo.Invoke(obj, parameters);
            }
            catch (Exception ex)
            {
                writer.Write((int)MessageResponse.ExecuteInvokeFailException);
                writer.Write(ex.Message);
                writer.Flush();
                return;
            }

            writer.Write((int)MessageResponse.ExecutedSuccessful);
            writer.Flush();
        }
    }
}