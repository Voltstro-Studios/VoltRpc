using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VoltRpc.IO;
/*
 * A lot of this code comes from Mirror's NetworkReader:
 * https://github.com/vis2k/Mirror/blob/98d7a9d7d12c4b965077d69e3eff5864bc9c79df/Assets/Mirror/Runtime/NetworkReader.cs
 *
 * Some code also comes from .NET Runtime's BufferedStream:
 * https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/IO/BufferedStream.cs
 */

/// <summary>
///     A buffered reader for a <see cref="Stream" />
/// </summary>
public class BufferedReader : IDisposable
{
    /// <summary>
    ///     The incoming <see cref="Stream" />
    /// </summary>
    protected readonly Stream IncomingStream;
    
    /// <summary>
    ///     Internal access to the underlining <see cref="UTF8Encoding"/> for <see cref="string"/>s
    /// </summary>
    internal readonly UTF8Encoding encoding;
    
    /// <summary>
    ///     Internal access to the underlining buffer
    /// </summary>
    internal byte[] buffer;

    /// <summary>
    ///     Current read length of the underlining <see cref="Stream"/>
    /// </summary>
    internal int readLength;

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
    public int Position { get; internal set; }

    /// <summary>
    ///     The length of the buffer
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length;
    }

    /// <summary>
    ///     You may need to override this if your <see cref="Stream"/> requires it
    /// </summary>
    protected virtual bool IncomingStreamNeedToAdjustPosition { get; } = false;

    /// <summary>
    ///     You may need to override this if your <see cref="Stream" /> requires it
    /// </summary>
    protected virtual long IncomingStreamPosition { get; set; }

    /// <summary>
    ///     Reads a blittable type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe T ReadBlittable<T>()
        where T : unmanaged
    {
        int size = sizeof(T);
        
        if(Position == readLength)
            ReadStream(size);

        if (Position + size > Length)
            throw new EndOfStreamException("Read stream out of range!");

        T value;
        fixed (byte* ptr = &buffer[Position])
        {
            value = *(T*)ptr;
        }

        Position += size;
        return value;
    }
    
    /// <summary>
    ///     Reads a <see cref="byte" />
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public byte ReadByte()
    {
        return ReadBlittable<byte>();
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
            ReadStream(count);

        //Check if within buffer limits
        if (Position + count > readLength)
        {
            //Attempt to read again
            ReadStream(count);
            if (Position + count > readLength)
                throw new EndOfStreamException("Cannot read beyond stream!");
        }

        //Return the segment
        ArraySegment<byte> result = new(buffer, Position, count);
        Position += count;
        return result;
    }
    
    /// <summary>
    ///     Reads a <see cref="string" />
    /// </summary>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public string ReadString()
    {
        //Read number of bytes
        ushort size = this.ReadUShort();

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

    /// <summary>
    ///     Reads more of the underlining <see cref="Stream"/>
    /// </summary>
    /// <exception cref="EndOfStreamException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ReadStream(int neededSize)
    {
        //Resize buffer if needed
        if (neededSize > Length)
            buffer = IoUtils.CreateBuffer(Length + neededSize);

        int totalReadLength = 0;
        if (IncomingStreamNeedToAdjustPosition)
            totalReadLength += Position;
        
        while (neededSize != 0)
        {
            //Keep reading until there is not more data, or we have reached our needed size
            int readSize = IncomingStream.Read(buffer, totalReadLength, neededSize);
            if(readSize == 0)
                break;

            neededSize -= readSize;
            totalReadLength += readSize;
        }

        //Still not enough data
        if (neededSize > totalReadLength)
            throw new EndOfStreamException();
        
        readLength = totalReadLength;
        
        if(!IncomingStreamNeedToAdjustPosition)
            Position = 0;
        
        if (readLength == 0)
            throw new EndOfStreamException();
    }
    
    #region Destroy

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
}