using System.IO;

namespace VoltRpc.IO
{
    /// <summary>
    ///     A buffered reader for a <see cref="Stream"/>
    /// </summary>
    public class BufferedReader
    {
        private readonly Stream incomingStream;
        
        private readonly byte[] buffer;
        private int position;
        private int readLength;
        
        internal BufferedReader(Stream incoming)
        {
            incomingStream = incoming;
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
                readLength = incomingStream.Read(buffer, 0, buffer.Length);
                position = 0;
                
                if (readLength == 0)
                    throw new EndOfStreamException();
            }

            return buffer[position++];
        }
    }
}