using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VoltRpc.IO
{
    /// <summary>
    ///     A buffered writer for a <see cref="Stream"/>
    /// </summary>
    public class BufferedWriter : IDisposable
    {
        /// <summary>
        ///     Max lenght for a <see cref="string"/>
        /// </summary>
        public const int MaxStringLength = 1024 * 32;
        
        private readonly Stream outputStream;
        private readonly UTF8Encoding encoding;

        private readonly byte[] stringBuffer;
        private byte[] buffer;
        private int position;
        
        /// <summary>
        ///     Creates a new <see cref="BufferedWriter"/> instance
        /// </summary>
        /// <param name="output"></param>
        internal BufferedWriter(Stream output)
        {
            outputStream = output;
            encoding = new UTF8Encoding(false, true);
            stringBuffer = new byte[MaxStringLength];
            buffer = new byte[8000];
        }

        /// <summary>
        ///     Reset position
        /// </summary>
        internal void Reset()
        {
            position = 0;
        }

        /// <summary>
        ///     Writes a <see cref="byte"/>
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            EnsureCapacity(position + 1);
            buffer[position++] = value;
        }
        
        /// <summary>
        ///     Writes an array of <see cref="byte"/>s
        /// </summary>
        /// <param name="bytesBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void WriteBytes(byte[] bytesBuffer, int offset, int count)
        {
            EnsureCapacity(position + count);
            Array.ConstrainedCopy(bytesBuffer, offset, buffer, position, count);
            position += count;
        }
        
        /// <summary>
        ///     Writes a <see cref="ushort"/>
        /// </summary>
        /// <param name="value"></param>
        public void WriteUShort(ushort value)
        {
            WriteByte((byte)value);
            WriteByte((byte)(value >> 8));
        }
        
        /// <summary>
        ///     Writes a <see cref="string"/>
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteUShort(0);
                return;
            }

            // write string with same method as NetworkReader
            // convert to byte[]
            int size = encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);

            // check if within max size
            if (size >= MaxStringLength)
                throw new IndexOutOfRangeException($"Cannot write string larger then {MaxStringLength}!");

            // write size and bytes
            WriteUShort(checked((ushort)(size + 1)));
            WriteBytes(stringBuffer, 0, size);
        }

        internal void Flush()
        {
            outputStream.Write(buffer, 0, position);
            outputStream.Flush();
            Reset();
        }
        
        /// <inheritdoc/>
        public void Dispose()
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int value)
        {
            if (buffer.Length < value)
            {
                int capacity = Math.Max(value, buffer.Length * 2);
                Array.Resize(ref buffer, capacity);
            }
        }
    }
}