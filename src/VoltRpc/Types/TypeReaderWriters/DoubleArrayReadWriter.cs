using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class DoubleArrayReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            double[] array = (double[])obj;
            if (array == null)
            {
                writer.WriteInt(-1);
                return;
            }
            
            writer.WriteInt(array.Length);
            foreach (double u in array)
                writer.WriteDouble(u);
        }

        public object Read(BufferedReader reader)
        {
            int size = reader.ReadInt();
            if (size == -1)
            {
                return null;
            }

            double[] array = new double[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = reader.ReadDouble();
            }

            return array;
        }
    }
}