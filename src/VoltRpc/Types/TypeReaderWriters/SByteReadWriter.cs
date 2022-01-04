using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class SByteReadWriter : TypeReadWriter<sbyte>
{
    public override void Write(BufferedWriter writer, sbyte obj)
    {
        writer.WriteSByte(obj);
    }

    public override sbyte Read(BufferedReader reader)
    {
        return reader.ReadSByte();
    }
}