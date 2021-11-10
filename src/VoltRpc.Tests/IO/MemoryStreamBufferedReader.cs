using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class MemoryStreamBufferedReader : BufferedReader
{
    internal MemoryStreamBufferedReader(MemoryStream incoming)
        : base(incoming)
    {
    }

    protected override long IncomingStreamPosition
    {
        get => IncomingStream.Position;
        set => IncomingStream.Position = value;
    }
}