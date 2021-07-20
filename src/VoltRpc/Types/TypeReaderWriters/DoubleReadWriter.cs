using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class DoubleReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteDouble((double)obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadDouble();
        }
    }
}