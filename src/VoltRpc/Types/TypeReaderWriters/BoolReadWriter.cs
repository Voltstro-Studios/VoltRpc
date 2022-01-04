using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class BoolReadWriter : TypeReadWriter<bool>
{
    public override void Write(BufferedWriter writer, bool obj)
    {
        writer.WriteBool(obj);
    }

    public override bool Read(BufferedReader reader)
    {
        return reader.ReadBool();
    }
}