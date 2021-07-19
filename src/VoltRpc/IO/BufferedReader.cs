using System;
using System.IO;

namespace VoltRpc.IO
{
    /// <summary>
    ///     A buffered reader for a <see cref="Stream"/>
    /// </summary>
    public class BufferedReader : IDisposable
    {
        /// <summary>
        ///     The incoming <see cref="Stream"/>
        /// </summary>
        protected readonly Stream IncomingStream;
        
        /// <summary>
        ///     You may need to override this if your <see cref="Stream"/> requires it
        /// </summary>
        protected virtual long IncomingStreamPosition
        {
            get;
            set;
        } 
        
        private readonly byte[] buffer;
        private int position;
        private int readLength;
        
        internal BufferedReader(Stream incoming)
        {
            IncomingStream = incoming;
            buffer = new byte[8000];
        }

        /// <summary>
        ///     Reads a <see cref="byte"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EndOfStreamException"></exception>
        public byte ReadByte()
        {
            if (position == readLength)
            {
                readLength = IncomingStream.Read(buffer, 0, buffer.Length);
                IncomingStreamPosition = 0;
                position = 0;
                
                if (readLength == 0)
                    throw new EndOfStreamException();
            }

            return buffer[position++];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            
        }
    }
}