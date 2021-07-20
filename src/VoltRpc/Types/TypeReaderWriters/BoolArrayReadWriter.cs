using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class BoolArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            bool[] array = (bool[])obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(array.Length);
            foreach (bool b in array)
                writer.WriteBool(b);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1)
            {
                return null;
            }

            bool[] array = new bool[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = reader.ReadBool();
            }

            return array;
        }
    }
}