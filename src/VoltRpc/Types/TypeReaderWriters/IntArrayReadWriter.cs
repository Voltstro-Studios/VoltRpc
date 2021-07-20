using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class IntArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            int[] array = (int[])obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(array.Length);
            foreach (int u in array)
                writer.WriteInt(u);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1)
            {
                return null;
            }

            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = reader.ReadInt();
            }

            return array;
        }
    }
}