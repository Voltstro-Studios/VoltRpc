using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class StringArrayReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        string[] array = (string[]) obj;
        if (array == null)
        {
            writer.WriteInt(-1);
            return;
        }

        writer.WriteInt(array.Length);
        foreach (string s in array)
            writer.WriteString(s);
    }

    public object Read(BufferedReader reader)
    {
        int size = reader.ReadInt();
        if (size == -1) return null;

        string[] array = new string[size];
        for (int i = 0; i < size; i++) array[i] = reader.ReadString();

        return array;
    }
}