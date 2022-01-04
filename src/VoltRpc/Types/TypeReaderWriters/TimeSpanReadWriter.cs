using System;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

internal sealed class TimeSpanReadWriter : TypeReadWriter<TimeSpan>
{
    public override void Write(BufferedWriter writer, TimeSpan value)
    {
        writer.WriteLong(value.Ticks);
    }

    public override TimeSpan Read(BufferedReader reader)
    {
        return TimeSpan.FromTicks(reader.ReadLong());
    }
}