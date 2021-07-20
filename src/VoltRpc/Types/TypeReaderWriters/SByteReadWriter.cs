using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class SByteReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteSByte((sbyte)obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadSByte();
        }
    }
}