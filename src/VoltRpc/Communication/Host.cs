using System;
using System.IO;
using System.Threading.Tasks;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Host"/> receives and responds to a <see cref="Client"/>'s requests
    /// </summary>
    public abstract class Host : IDisposable
    {
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
                        break;
                    case MessageType.Shutdown:
                        doContinue = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


            } while (doContinue);
            
            binReader.Dispose();
            binWriter.Dispose();
        }
    }
}