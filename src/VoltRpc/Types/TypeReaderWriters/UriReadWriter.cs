using System;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class UriReadWriter : TypeReadWriter<Uri>
{
    public override void Write(BufferedWriter writer, Uri value)
    {
        writer.WriteString(value?.ToString());
    }

    public override Uri Read(BufferedReader reader)
    {
        string url = reader.ReadString();
        return url == null ? null : new Uri(url);
    }
}