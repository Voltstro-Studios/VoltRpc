using System;
using System.IO;
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

        private TypeReaderWriterManager typeReaderWriterManager;

        /// <summary>
        ///     Creates a new <see cref="Client"/> instance
        /// </summary>
        protected Client()
        {
            typeReaderWriterManager = new TypeReaderWriterManager();
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
        public void InvokeMethod(string methodName, params object[] parameters)
        {
            //Write the method name first
            binWriter.Write((int) MessageType.InvokeMethod);
            binWriter.Write(methodName);

            //Now we need to write our parameters
            WriteParams(parameters);

            binWriter.Flush();

            //Get response
            MessageResponse response = (MessageResponse) binReader.ReadInt32();
            if (response == MessageResponse.NoMethodFound)
                throw new MissingMethodException("The method does not exist on the server!");
            if (response == MessageResponse.ExecuteFailNoTypeReader)
                throw new NoTypeReaderWriterException();
            if (response == MessageResponse.ExecuteTypeReaderFail)
            {
                string reason = binReader.ReadString();
                throw new Exception($"Type reader failed to read: {reason}");
            }
            if (response == MessageResponse.ExecuteInvokeFailException)
            {
                string reason = binReader.ReadString();
                throw new Exception($"The method failed for some reason: {reason}");
            }
        }

        private void WriteParams(object[] parameters)
        {
            binWriter.Write(parameters.Length);

            foreach (object parameter in parameters)
            {
                ITypeReadWriter writer = typeReaderWriterManager.GetType(parameter);
                
                //TODO: We should cache stuff like this
                binWriter.Write(parameter.GetType().FullName);
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