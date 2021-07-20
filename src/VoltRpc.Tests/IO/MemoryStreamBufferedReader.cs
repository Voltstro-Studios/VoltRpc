﻿using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO
{
    public class MemoryStreamBufferedReader : BufferedReader
    {
        protected override long IncomingStreamPosition
        {
            get => IncomingStream.Position;
            set => IncomingStream.Position = value;
        }

        internal MemoryStreamBufferedReader(MemoryStream incoming) 
            : base(incoming)
        {
        }
    }
}