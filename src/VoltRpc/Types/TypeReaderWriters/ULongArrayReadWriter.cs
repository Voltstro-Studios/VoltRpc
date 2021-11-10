using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class ULongArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        ulong[] array = (ulong[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (ulong u in array)
            writer.WriteULong(u);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        ulong[] array = new ulong[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadULong();

        return array;
    }
}