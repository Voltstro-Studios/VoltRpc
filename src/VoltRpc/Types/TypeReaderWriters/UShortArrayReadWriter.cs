using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UShortArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        ushort[] array = (ushort[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (ushort us in array)
            writer.WriteUShort(us);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        ushort[] array = new ushort[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadUShort();

        return array;
    }
}