using System;
using System.IO;
using System.Text;

namespace VoltRpc.IO;
/*
 * Base of this code comes from Mirror's NetworkReader:
 * https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkReader.cs
 *
 * Some code also comes from .NET Runtime's BufferedStream:
 * https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/IO/BufferedStream.cs
 */

/// <summary>
///     A buffered reader for a <see cref="Stream" />
/// </summary>
public class BufferedReader : IDisposable
{
    private readonly byte[] buffer;

    private readonly UTF8Encoding encoding;

    /// <summary>
    ///     The incoming <see cref="Stream" />
    /// </summary>
    protected readonly Stream IncomingStream;

    private int readLength;

    /// <summary>
    ///     Creates a new <see cref="BufferedReader" /> instance
    /// </summary>
    /// <param name="incoming"></param>
    /// <param name="bufferSize"></param>
    public BufferedReader(Stream incoming, int bufferSize = 8000)
    {
        IncomingStream = incoming;
        encoding = new UTF8Encoding(false, true);
        buffer = IoUtils.CreateBuffer(bufferSize);
    }

    /// <summary>
    ///     The current position of the buffer
    /// </summary>
    public int Position { get; private set; }

    /// <summary>
    ///     The length of the buffer
    /// </summary>
    public int Length => buffer.Length;

    /// <summary>
    ///     You may need to override this if your <see cref="Stream" /> requires it
    /// </summary>
    protected virtual long IncomingStreamPosition { get; set; }

    #region Destroy

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion

    /// <summary>
    ///     Reads a <see cref="byte" />
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public byte ReadByte()
    {
        if (Position == readLength)
            ReadStream();

        return buffer[Position++];
    }

    /// <summary>
    ///     Reads an array of <see cref="byte" />s as an <see cref="ArraySegment{T}" />
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public ArraySegment<byte> ReadBytesSegment(int count)
    {
        if (Position == readLength)
            ReadStream();

        //Check if within buffer limits
        if (Position + count > readLength)
        {
            //Attempt to read again
            ReadStream();
            if (Position + count > readLength)
                throw new EndOfStreamException("Cannot read beyond stream!");
        }

        //Return the segment
        ArraySegment<byte> result = new(buffer, Position, count);
        Position += count;
        return result;
    }

    /// <summary>
    ///     Reads a <see cref="sbyte" />
    /// </summary>
    /// <returns></returns>
    public sbyte ReadSByte()
    {
        return (sbyte) ReadByte();
    }

    /// <summary>
    ///     Reads a <see cref="bool" />
    /// </summary>
    /// <returns></returns>
    public bool ReadBool()
    {
        return ReadByte() != 0;
    }

    /// <summary>
    ///     Reads a <see cref="ushort" />
    /// </summary>
    /// <returns></returns>
    public ushort ReadUShort()
    {
        ushort value = 0;
        value |= ReadByte();
        value |= (ushort) (ReadByte() << 8);
        return value;
    }

    /// <summary>
    ///     Reads a <see cref="short" />
    /// </summary>
    /// <returns></returns>
    public short ReadShort()
    {
        return (short) ReadUShort();
    }

    /// <summary>
    ///     Reads a <see cref="char" />
    /// </summary>
    /// <returns></returns>
    public char ReadChar()
    {
        return (char) ReadUShort();
    }

    /// <summary>
    ///     Reads a <see cref="uint" />
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt()
    {
        uint value = 0;
        value |= ReadByte();
        value |= (uint) (ReadByte() << 8);
        value |= (uint) (ReadByte() << 16);
        value |= (uint) (ReadByte() << 24);
        return value;
    }

    /// <summary>
    ///     Reads a <see cref="int" />
    /// </summary>
    /// <returns></returns>
    public int ReadInt()
    {
        return (int) ReadUInt();
    }

    /// <summary>
    ///     Reads a <see cref="ulong" />
    /// </summary>
    /// <returns></returns>
    public ulong ReadULong()
    {
        ulong value = 0;
        value |= ReadByte();
        value |= (ulong) ReadByte() << 8;
        value |= (ulong) ReadByte() << 16;
        value |= (ulong) ReadByte() << 24;
        value |= (ulong) ReadByte() << 32;
        value |= (ulong) ReadByte() << 40;
        value |= (ulong) ReadByte() << 48;
        value |= (ulong) ReadByte() << 56;
        return value;
    }

    /// <summary>
    ///     Reads a <see cref="long" />
    /// </summary>
    /// <returns></returns>
    public long ReadLong()
    {
        return (long) ReadULong();
    }

    /// <summary>
    ///     Reads a <see cref="float" />
    /// </summary>
    /// <returns></returns>
    public float ReadFloat()
    {
        UIntFloat converter = new()
        {
            intValue = ReadUInt()
        };
        return converter.floatValue;
    }

    /// <summary>
    ///     Reads a <see cref="double" />
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        UIntDouble converter = new()
        {
            longValue = ReadULong()
        };
        return converter.doubleValue;
    }

    /// <summary>
    ///     Reads a <see cref="decimal" />
    /// </summary>
    /// <returns></returns>
    public decimal ReadDecimal()
    {
        UIntDecimal converter = new()
        {
            longValue1 = ReadULong(),
            longValue2 = ReadULong()
        };
        return converter.decimalValue;
    }

    /// <summary>
    ///     Reads a <see cref="string" />
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
            throw new EndOfStreamException(
                $"Read string was too long! Max size is {BufferedWriter.MaxStringLength}.");

        ArraySegment<byte> data = ReadBytesSegment(realSize);

        //Convert directly from buffer to string via encoding
        return encoding.GetString(data.Array, data.Offset, data.Count);
    }

    private void ReadStream()
    {
        readLength = IncomingStream.Read(buffer, 0, buffer.Length);
        IncomingStreamPosition = 0;
        Position = 0;

        if (readLength == 0)
            throw new EndOfStreamException();
    }
}