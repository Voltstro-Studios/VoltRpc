using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class ShortReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        writer.WriteShort((short) obj);
    }

    public object Read(BufferedReader reader)
    {
        return reader.ReadShort();
    }
}