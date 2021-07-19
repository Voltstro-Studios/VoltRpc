using System;
using System.IO;
using System.Text;

namespace VoltRpc.IO
{
    /*
     * Base of this code comes from Mirror's NetworkReader:
     * https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkReader.cs
     *
     * Some code also comes from .NET Runtime's BufferedStream:
     * https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/IO/BufferedStream.cs
     */

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
        
        private readonly UTF8Encoding encoding;
        private readonly byte[] buffer;
        
        private int position;
        private int readLength;
        
        internal BufferedReader(Stream incoming)
        {
            IncomingStream = incoming;
            encoding = new UTF8Encoding(false, true);
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
                ReadStream();
            
            return buffer[position++];
        }
        
        /// <summary>
        ///     Reads an array of <see cref="byte"/>s as an <see cref="ArraySegment{T}"/>
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <exception cref="EndOfStreamException"></exception>
        public ArraySegment<byte> ReadBytesSegment(int count)
        {
            if (position == readLength)
                ReadStream();

            //Check if within buffer limits
            if (position + count > readLength)
            {
                //Attempt to read again
                ReadStream();
                if(position + count > readLength)
                    throw new EndOfStreamException("ReadBytesSegment can't read " + count + " bytes because it would read past the end of the stream.");
            }

            //Return the segment
            ArraySegment<byte> result = new ArraySegment<byte>(buffer, position, count);
            position += count;
            return result;
        }
        
        /// <summary>
        ///     Reads a <see cref="ushort"/>
        /// </summary>
        /// <returns></returns>
        public ushort ReadUShort()
        {
            ushort value = 0;
            value |= ReadByte();
            value |= (ushort)(ReadByte() << 8);
            return value;
        }
        
        /// <summary>
        ///     Reads a <see cref="uint"/>
        /// </summary>
        /// <returns></returns>
        public uint ReadUInt()
        {
            uint value = 0;
            value |= ReadByte();
            value |= (uint)(ReadByte() << 8);
            value |= (uint)(ReadByte() << 16);
            value |= (uint)(ReadByte() << 24);
            return value;
        }

        /// <summary>
        ///     Reads a <see cref="int"/>
        /// </summary>
        /// <returns></returns>
        public int ReadInt() => (int) ReadUInt();

        /// <summary>
        ///     Reads a <see cref="string"/>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="EndOfStreamException"></exception>
        public string ReadString()
        {
            //Read number of bytes
            ushort size = ReadUShort();

            //Null support
            if (size == 0)
                return null;

            int realSize = size - 1;

            //Make sure it's within limits to avoid allocation attacks etc.
            if (realSize >= BufferedWriter.MaxStringLength)
            {
                throw new EndOfStreamException("ReadString too long: " + realSize + ". Limit is: " + BufferedWriter.MaxStringLength);
            }

            ArraySegment<byte> data = ReadBytesSegment(realSize);

            //Convert directly from buffer to string via encoding
            return encoding.GetString(data.Array, data.Offset, data.Count);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        private void ReadStream()
        {
            readLength = IncomingStream.Read(buffer, 0, buffer.Length);
            IncomingStreamPosition = 0;
            position = 0;
                
            if (readLength == 0)
                throw new EndOfStreamException();
        }
    }
}