using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class ULongReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteULong((ulong)obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadULong();
        }
    }
}