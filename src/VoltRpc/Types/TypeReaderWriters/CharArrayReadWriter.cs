using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class CharArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            char[] array = (char[])obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(array.Length);
            foreach (char c in array)
                writer.WriteChar(c);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1)
            {
                return null;
            }

            char[] array = new char[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = reader.ReadChar();
            }

            return array;
        }
    }
}