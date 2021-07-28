﻿using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class DecimalReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteDecimal((decimal) obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadDecimal();
        }
    }
}