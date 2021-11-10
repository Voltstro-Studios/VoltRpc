using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UIntReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        writer.WriteUInt((uint) obj);
    }

    public object Read(BufferedReader reader)
    {
        return reader.ReadUInt();
    }
}