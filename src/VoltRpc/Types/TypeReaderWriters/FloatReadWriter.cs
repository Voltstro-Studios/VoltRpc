using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class FloatReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        writer.WriteFloat((float) obj);
    }

    public object Read(BufferedReader reader)
    {
        return reader.ReadFloat();
    }
}