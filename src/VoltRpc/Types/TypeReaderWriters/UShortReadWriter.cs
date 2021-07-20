using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class UShortReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteUShort((ushort)obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadUShort();
        }
    }
}