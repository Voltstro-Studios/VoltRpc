using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class FloatReadWriter : TypeReadWriter<float>
{
    public override void Write(BufferedWriter writer, float obj)
    {
        writer.WriteFloat(obj);
    }

    public override float Read(BufferedReader reader)
    {
        return reader.ReadFloat();
    }
}