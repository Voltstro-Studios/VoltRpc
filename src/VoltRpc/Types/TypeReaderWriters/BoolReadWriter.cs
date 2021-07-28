using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class BoolReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteBool((bool) obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadBool();
        }
    }
}