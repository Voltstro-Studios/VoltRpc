using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VoltRpc.IO;
/*
 * Base of this code comes from Mirror's NetworkWriter:
 * https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkWriter.cs
 */

/// <summary>
///     A buffered writer for a <see cref="Stream" />
/// </summary>
public class BufferedWriter : IDisposable
{
    /// <summary>
    ///     Max length for a <see cref="string" />
    /// </summary>
    public const int MaxStringLength = 1024 * 32;
    
    /// <summary>
    ///     Output <see cref="Stream" />
    /// </summary>
    protected readonly Stream OutputStream;

    private readonly byte[] stringBuffer;
    
    internal readonly UTF8Encoding encoding;
    internal byte[] buffer;

    /// <summary>
    ///     Creates a new <see cref="BufferedWriter" /> instance
    /// </summary>
    /// <param name="output"></param>
    /// <param name="bufferSize"></param>
    public BufferedWriter(Stream output, int bufferSize = 8000)
    {
        OutputStream = output;
        encoding = new UTF8Encoding(false, true);
        stringBuffer = new byte[MaxStringLength];
        buffer = IoUtils.CreateBuffer(bufferSize);
    }

    /// <summary>
    ///     The current position of the buffer
    /// </summary>
    public int Position { get; internal set; }

    /// <summary>
    ///     The length of the buffer
    /// </summary>
    public int Length => buffer.Length;

    /// <summary>
    ///     You may need to override this if your <see cref="Stream" /> requires it
    /// </summary>
    protected virtual long OutputStreamPosition { get; set; }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Reset position
    /// </summary>
    public void Reset()
    {
        OutputStreamPosition = 0;
        Position = 0;
    }

    /// <summary>
    ///     Writes a <see cref="byte" />
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <param name="ensureCapacity">
    ///     Checks that capacity of the buffer is larger enough.
    ///     This should ALWAYS be enabled, UNLESS you are doing multiple <see cref="WriteByte"/> calls.
    ///     <para>
    ///         If you are doing multiple <see cref="WriteByte"/> calls,
    ///         then only do a capacity check on the first one, and set capacitySize to the total size.
    ///     </para>
    /// </param>
    /// <param name="capacitySize">Capacity size of the buffer to check</param>
    public void WriteByte(byte value, bool ensureCapacity = true, int capacitySize = 1)
    {
        if(ensureCapacity)
            EnsureCapacity(Position + capacitySize);
        
        buffer[Position++] = value;
    }

    /// <summary>
    ///     Writes an array of <see cref="byte" />s
    /// </summary>
    /// <param name="bytesBuffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public void WriteBytes(byte[] bytesBuffer, int offset, int count)
    {
        EnsureCapacity(Position + count);
        Array.ConstrainedCopy(bytesBuffer, offset, buffer, Position, count);
        Position += count;
    }

    /// <summary>
    ///     Writes a <see cref="sbyte" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteSByte(sbyte value)
    {
        WriteByte((byte) value);
    }

    /// <summary>
    ///     Writes a <see cref="bool" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteBool(bool value)
    {
        WriteByte((byte) (value ? 1 : 0));
    }

    /// <summary>
    ///     Writes a <see cref="ushort" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteUShort(ushort value)
    {
        WriteByte((byte) value, true, 2);
        WriteByte((byte) (value >> 8), false);
    }

    /// <summary>
    ///     Writes a <see cref="short" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteShort(short value)
    {
        WriteUShort((ushort) value);
    }

    /// <summary>
    ///     Writes a <see cref="char" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteChar(char value)
    {
        WriteUShort(value);
    }

    /// <summary>
    ///     Writes a <see cref="uint" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteUInt(uint value)
    {
        WriteByte((byte) value, true, 4);
        WriteByte((byte) (value >> 8), false);
        WriteByte((byte) (value >> 16), false);
        WriteByte((byte) (value >> 24), false);
    }

    /// <summary>
    ///     Writes a <see cref="int" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteInt(int value)
    {
        WriteUInt((uint) value);
    }

    /// <summary>
    ///     Writes a <see cref="ulong" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteULong(ulong value)
    {
        WriteByte((byte) value, true, 8);
        WriteByte((byte) (value >> 8), false);
        WriteByte((byte) (value >> 16), false);
        WriteByte((byte) (value >> 24), false);
        WriteByte((byte) (value >> 32), false);
        WriteByte((byte) (value >> 40), false);
        WriteByte((byte) (value >> 48), false);
        WriteByte((byte) (value >> 56), false);
    }

    /// <summary>
    ///     Writes a <see cref="long" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteLong(long value)
    {
        WriteULong((ulong) value);
    }

    /// <summary>
    ///     Writes a <see cref="float" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteFloat(float value)
    {
        UIntFloat converter = new()
        {
            floatValue = value
        };
        WriteUInt(converter.intValue);
    }

    /// <summary>
    ///     Writes a <see cref="double" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteDouble(double value)
    {
        UIntDouble converter = new()
        {
            doubleValue = value
        };
        WriteULong(converter.longValue);
    }

    /// <summary>
    ///     Writes a <see cref="decimal" />
    /// </summary>
    /// <param name="value"></param>
    public void WriteDecimal(decimal value)
    {
        // the only way to read it without allocations is to both read and
        // write it with the FloatConverter (which is not binary compatible
        // to writer.Write(decimal), hence why we use it here too)
        UIntDecimal converter = new()
        {
            decimalValue = value
        };
        WriteULong(converter.longValue1);
        WriteULong(converter.longValue2);
    }

    /// <summary>
    ///     Writes a <see cref="string" />
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
        WriteUShort(checked((ushort) (size + 1)));
        WriteBytes(stringBuffer, 0, size);
    }

    /// <summary>
    ///     Writes the buffer to the out <see cref="Stream" />
    /// </summary>
    public void Flush()
    {
        OutputStream.Write(buffer, 0, Position);
        OutputStream.Flush();
        OutputStreamPosition = 0;
        Reset();
    }

    /// <summary>
    ///     Ensures the buffer's capacity is large enough to write the size
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnsureCapacity(int value)
    {
        if (buffer.Length < value)
        {
            int capacity = Math.Max(value, buffer.Length * 2);

            //Create new buffer and copy the contents of the old one to it
            byte[] newBuffer = IoUtils.CreateBuffer(capacity);
            Array.Copy(buffer, newBuffer, buffer.Length);
            buffer = newBuffer;
        }
    }
}