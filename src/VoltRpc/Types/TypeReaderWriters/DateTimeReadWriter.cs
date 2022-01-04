using System;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class DateTimeReadWriter : TypeReadWriter<DateTime>
{
    public override void Write(BufferedWriter writer, DateTime value)
    {
        writer.WriteLong(value.ToBinary());
    }

    public override DateTime Read(BufferedReader reader)
    {
        return DateTime.FromBinary(reader.ReadLong());
    }
}