using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class ByteArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            byte[] array = (byte[]) obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }

            writer.WriteInt(array.Length);
            foreach (byte b in array)
                writer.WriteByte(b);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1) return null;

            byte[] array = new byte[size];
            for (int i = 0; i < size; i++) array[i] = reader.ReadByte();

            return array;
        }
    }
}