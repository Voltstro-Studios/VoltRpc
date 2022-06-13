using VoltRpc.Extension.Memory;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Benchmarks.Core.FastArray;

public sealed class FastArrayTypeReaderWriter : TypeReadWriter<FastArrayContainer>
{
    public override void Write(BufferedWriter writer, FastArrayContainer value)
    {
        int size = value.Data.Length;
        writer.WriteInt(size);
        
        writer.WriteBytes(value.Data, 0, size);
    }

    public override FastArrayContainer Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        reader.ReadBytesSpanSlice(size);
        
        //NOTE: Normally you would want to copy the data to the FastArrayContainer.Data, but in this case we won't
        return new FastArrayContainer();
    }
}