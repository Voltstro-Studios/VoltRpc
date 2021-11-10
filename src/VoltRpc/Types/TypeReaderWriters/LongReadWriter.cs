using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class LongReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        writer.WriteLong((long) obj);
    }

    public object Read(BufferedReader reader)
    {
        return reader.ReadLong();
    }
}