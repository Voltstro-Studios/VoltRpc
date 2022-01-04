using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UIntReadWriter : TypeReadWriter<uint>
{
    public override void Write(BufferedWriter writer, uint obj)
    {
        writer.WriteUInt(obj);
    }

    public override uint Read(BufferedReader reader)
    {
        return reader.ReadUInt();
    }
}