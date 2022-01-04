using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class IntReadWriter : TypeReadWriter<int>
{
    public override void Write(BufferedWriter writer, int obj)
    {
        writer.WriteInt(obj);
    }

    public override int Read(BufferedReader reader)
    {
        return reader.ReadInt();
    }
}