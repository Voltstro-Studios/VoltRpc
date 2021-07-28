using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class ByteReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteByte((byte) obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadByte();
        }
    }
}