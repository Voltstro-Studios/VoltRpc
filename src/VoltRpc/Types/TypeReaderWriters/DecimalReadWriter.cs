using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class DecimalReadWriter : TypeReadWriter<decimal>
{
    public override void Write(BufferedWriter writer, decimal obj)
    {
        writer.WriteDecimal(obj);
    }

    public override decimal Read(BufferedReader reader)
    {
        return reader.ReadDecimal();
    }
}