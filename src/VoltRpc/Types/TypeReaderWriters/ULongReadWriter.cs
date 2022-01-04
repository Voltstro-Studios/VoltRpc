using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class ULongReadWriter : TypeReadWriter<ulong>
{
    public override void Write(BufferedWriter writer, ulong obj)
    {
        writer.WriteULong(obj);
    }

    public override ulong Read(BufferedReader reader)
    {
        return reader.ReadULong();
    }
}