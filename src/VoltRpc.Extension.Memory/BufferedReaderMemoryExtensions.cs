using System;
using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Extension.Memory;

/// <summary>
///     Provides extensions to <see cref="BufferedReader"/> 
/// </summary>
public static class BufferedReaderMemoryExtensions
{
    /// <summary>
    ///     Reads from <see cref="BufferedReader"/> and returns a <see cref="Span{T}"/> slice of the buffer
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> ReadBytesSpanSlice(this BufferedReader reader, int size)
    {
        if (reader.Position == reader.readLength)
            reader.ReadStream(size);

        //Check if within buffer limits
        if (reader.Position + size > reader.readLength)
        {
            //Attempt to read again
            reader.ReadStream(size);
            if (reader.Position + size > reader.readLength)
                throw new EndOfStreamException("Cannot read beyond stream!");
        }
        
        Span<byte> buffer = reader.buffer;

        Span<byte> data = buffer.Slice(reader.Position, size);
        reader.Position += size;
        return data;
    }

    /// <summary>
    ///     Reads from <see cref="BufferedReader"/> and returns a <see cref="Span{T}"/> copy of it
    ///     <para>This method does allocate</para>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> ReadBytesSpanCopy(this BufferedReader reader, int size)
    {
        ReadOnlySpan<byte> slice = reader.ReadBytesSpanSlice(size);

        Span<byte> copy = new byte[size];
        slice.CopyTo(copy);
        
        return copy;
    }

    /// <summary>
    ///     Reads a <see cref="string"/>, but is using <see cref="ReadBytesSpanSlice"/> instead
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public static string? ReadStringSpan(this BufferedReader reader)
    {
        ushort size = reader.ReadUShort();
        if (size == 0)
            return null;
        
        int realSize = size - 1;
        
        //Make sure it's within limits to avoid allocation attacks etc.
        if (realSize >= BufferedWriter.MaxStringLength)
            throw new EndOfStreamException(
                $"Read string was too long! Max size is {BufferedWriter.MaxStringLength}.");

        ReadOnlySpan<byte> data = reader.ReadBytesSpanSlice(realSize);

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        return reader.encoding.GetString(data);
#else
        return reader.encoding.GetString(data.ToArray());
#endif
    }
}