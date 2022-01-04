using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class StringReadWriter : TypeReadWriter<string>
{
    public override void Write(BufferedWriter writer, string obj)
    {
        writer.WriteString(obj);
    }
    
    public override string Read(BufferedReader reader)
    {
        return reader.ReadString();
    }
}