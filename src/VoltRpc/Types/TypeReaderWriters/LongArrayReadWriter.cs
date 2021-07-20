using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class LongArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            long[] array = (long[])obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(array.Length);
            foreach (long u in array)
                writer.WriteLong(u);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1)
            {
                return null;
            }

            long[] array = new long[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = reader.ReadLong();
            }

            return array;
        }
    }
}