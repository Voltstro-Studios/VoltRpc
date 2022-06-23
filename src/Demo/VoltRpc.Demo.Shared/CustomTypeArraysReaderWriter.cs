using System;
using VoltRpc.Extension.Memory;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Demo.Shared;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for <see cref="CustomTypeArrays"/>
/// </summary>
public class CustomTypeArraysReaderWriter : TypeReadWriter<CustomTypeArrays>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, CustomTypeArrays value)
    {
        writer.WriteInt(value.Array.Length);
        writer.WriteBytes(value.Array, 0, value.Array.Length);
    }

    /// <inheritdoc />
    public override CustomTypeArrays Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        ReadOnlySpan<byte> data = reader.ReadBytesSpanSlice(size);
        return new CustomTypeArrays
        {
            Array = data.ToArray()
        };
    }
}