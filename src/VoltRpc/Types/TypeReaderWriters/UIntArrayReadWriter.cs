using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UIntArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        uint[] array = (uint[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (uint u in array)
            writer.WriteUInt(u);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        uint[] array = new uint[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadUInt();

        return array;
    }
}