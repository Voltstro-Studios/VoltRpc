using System;
using System.IO;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Client"/> sends messages to a <see cref="Host"/>
    /// </summary>
    public abstract class Client : IDisposable
    {
        private BinaryReader binReader;
        private BinaryWriter binWriter;

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

        /// <summary>
        ///     Sends the init message to the server
        /// </summary>
        /// <param name="stream"></param>
        protected void Initialize(Stream stream)
        {
            binReader = new BinaryReader(stream);
            binWriter = new BinaryWriter(stream);
        }
    }
}