using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class DecimalArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        decimal[] array = (decimal[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (decimal u in array)
            writer.WriteDecimal(u);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        decimal[] array = new decimal[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadDecimal();

        return array;
    }
}