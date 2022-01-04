using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class CharReadWriter : TypeReadWriter<char>
{
    public override void Write(BufferedWriter writer, char obj)
    {
        writer.WriteChar(obj);
    }

    public override char Read(BufferedReader reader)
    {
        return reader.ReadChar();
    }
}