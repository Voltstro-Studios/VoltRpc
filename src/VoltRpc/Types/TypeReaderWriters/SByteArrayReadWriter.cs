using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class SByteArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        sbyte[] array = (sbyte[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (sbyte b in array)
            writer.WriteSByte(b);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        sbyte[] array = new sbyte[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadSByte();

        return array;
    }
}