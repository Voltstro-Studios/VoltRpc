﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VoltRpc.IO
{
    /*
     * Base of this code comes from Mirror's NetworkWriter:
     * https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkWriter.cs
     */
    
    /// <summary>
    ///     A buffered writer for a <see cref="Stream"/>
    /// </summary>
    public class BufferedWriter : IDisposable
    {
        /// <summary>
        ///     Max length for a <see cref="string"/>
        /// </summary>
        public const int MaxStringLength = 1024 * 32;
        
        /// <summary>
        ///     Output <see cref="Stream"/>
        /// </summary>
        protected readonly Stream OutputStream;
        
        /// <summary>
        ///     You may need to override this if your <see cref="Stream"/> requires it
        /// </summary>
        protected virtual long OutputStreamPosition
        {
            get;
            set;
        }
        
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
            OutputStream = output;
            encoding = new UTF8Encoding(false, true);
            stringBuffer = new byte[MaxStringLength];
            buffer = new byte[8000];
        }

        /// <summary>
        ///     Reset position
        /// </summary>
        internal void Reset()
        {
            OutputStreamPosition = 0;
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
        ///     Writes a <see cref="uint"/>
        /// </summary>
        /// <param name="value"></param>
        public void WriteUInt(uint value)
        {
            WriteByte((byte)value);
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value >> 16));
            WriteByte((byte)(value >> 24));
        }

        /// <summary>
        ///     Writes a <see cref="int"/>
        /// </summary>
        /// <param name="value"></param>
        public void WriteInt(int value) => WriteUInt((uint) value);

        /// <summary>
        ///     Writes a <see cref="string"/>
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void WriteString(string value)
        {
            //Null support
            if (value == null)
            {
                WriteUShort(0);
                return;
            }
            
            //Write to string buffer
            int size = encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);

            //Check if within max size
            if (size >= MaxStringLength)
                throw new IndexOutOfRangeException($"Cannot write string larger then {MaxStringLength}!");

            //Write size and bytes
            WriteUShort(checked((ushort)(size + 1)));
            WriteBytes(stringBuffer, 0, size);
        }

        internal void Flush()
        {
            OutputStream.Write(buffer, 0, position);
            OutputStream.Flush();
            OutputStreamPosition = 0;
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