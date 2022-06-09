using System;
using VoltRpc.Extension.Memory;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Demo.Shared;

public class CustomTypeArraysReaderWriter : TypeReadWriter<CustomTypeArrays>
{
    public override void Write(BufferedWriter writer, CustomTypeArrays value)
    {
        writer.WriteInt(value.LargeArray.Length);
        writer.WriteBytes(value.LargeArray, 0, value.LargeArray.Length);
    }

    public override CustomTypeArrays Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        ReadOnlySpan<byte> data = reader.ReadBytesSpanSlice(size);
        return new CustomTypeArrays
        {
            LargeArray = data.ToArray()
        };
    }
}