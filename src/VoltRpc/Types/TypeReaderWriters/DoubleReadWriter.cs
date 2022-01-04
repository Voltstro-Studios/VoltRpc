using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class DoubleReadWriter : TypeReadWriter<double>
{
    public override void Write(BufferedWriter writer, double obj)
    {
        writer.WriteDouble(obj);
    }

    public override double Read(BufferedReader reader)
    {
        return reader.ReadDouble();
    }
}