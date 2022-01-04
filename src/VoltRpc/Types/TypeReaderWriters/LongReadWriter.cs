using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class LongReadWriter : TypeReadWriter<long>
{
    public override void Write(BufferedWriter writer, long obj)
    {
        writer.WriteLong(obj);
    }

    public override long Read(BufferedReader reader)
    {
        return reader.ReadLong();
    }
}