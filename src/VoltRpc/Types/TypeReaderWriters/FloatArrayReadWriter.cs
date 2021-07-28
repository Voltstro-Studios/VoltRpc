using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class FloatArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            float[] array = (float[]) obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }

            writer.WriteInt(array.Length);
            foreach (float u in array)
                writer.WriteFloat(u);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1) return null;

            float[] array = new float[size];
            for (int i = 0; i < size; i++) array[i] = reader.ReadFloat();

            return array;
        }
    }
}