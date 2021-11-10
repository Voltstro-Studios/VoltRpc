using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class IntReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        writer.WriteInt((int) obj);
    }

    public object Read(BufferedReader reader)
    {
        return reader.ReadInt();
    }
}