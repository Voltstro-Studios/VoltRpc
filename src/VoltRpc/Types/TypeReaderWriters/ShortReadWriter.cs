using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class ShortReadWriter : TypeReadWriter<short>
{
    public override void Write(BufferedWriter writer, short obj)
    {
        writer.WriteShort(obj);
    }

    public override short Read(BufferedReader reader)
    {
        return reader.ReadShort();
    }
}