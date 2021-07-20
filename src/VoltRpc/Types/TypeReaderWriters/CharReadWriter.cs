﻿using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    internal sealed class CharReadWriter : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
            writer.WriteChar((char)obj);
        }

        public object Read(BufferedReader reader)
        {
            return reader.ReadChar();
        }
    }
}