using System;
using VoltRpc.IO;

namespace VoltRpc.Extension.Memory;

/// <summary>
///     Provides extensions to <see cref="BufferedWriter"/>
/// </summary>
public static class BufferedWriterMemoryExtensions
{
    /// <summary>
    ///     Writes a <see cref="Span{T}"/> <see cref="byte"/> to a <see cref="BufferedWriter"/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    public static void WriteBytesSpan(this BufferedWriter writer, ReadOnlySpan<byte> value)
    {
        writer.CheckDispose();
        
        writer.EnsureCapacity(writer.Position + value.Length);
        
        value.CopyTo(writer.buffer);
        writer.Position += value.Length;
    }

    /// <summary>
    ///     Writes a <see cref="Memory{T}"/> <see cref="byte"/> to a <see cref="BufferedWriter"/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    public static void WriteBytesMemory(this BufferedWriter writer, ReadOnlyMemory<byte> value)
    {
        writer.WriteBytesSpan(value.Span);
    }
}