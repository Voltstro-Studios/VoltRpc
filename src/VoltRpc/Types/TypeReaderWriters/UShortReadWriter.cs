using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UShortReadWriter : TypeReadWriter<ushort>
{
    public override void Write(BufferedWriter writer, ushort obj)
    {
        writer.WriteUShort(obj);
    }

    public override ushort Read(BufferedReader reader)
    {
        return reader.ReadUShort();
    }
}