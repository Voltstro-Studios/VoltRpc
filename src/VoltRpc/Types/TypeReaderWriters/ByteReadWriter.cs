using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class ByteReadWriter : TypeReadWriter<byte>
{
    public override void Write(BufferedWriter writer, byte obj)
    {
        writer.WriteByte(obj);
    }

    public override byte Read(BufferedReader reader)
    {
        return reader.ReadByte();
    }
}