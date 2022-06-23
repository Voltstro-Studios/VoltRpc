using System;
using System.Linq;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class GuidReadWriter : TypeReadWriter<Guid>
{
    private const int GuidSize = 16;
    
    public override void Write(BufferedWriter writer, Guid value)
    {
        byte[] data = value.ToByteArray();
        writer.WriteBytes(data, 0, GuidSize);
    }

    public override Guid Read(BufferedReader reader)
    {
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        ArraySegment<byte> data = reader.ReadBytesSegment(GuidSize);
#else
        byte[] data = reader.ReadBytesSegment(GuidSize).ToArray();
#endif
        if (data == null)
            throw new NullReferenceException("Guid data was null!");

        return new Guid(data);
    }
}