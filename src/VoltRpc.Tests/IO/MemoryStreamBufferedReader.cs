using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class MemoryStreamBufferedReader : BufferedReader
{
    internal MemoryStreamBufferedReader(MemoryStream incoming, int size)
        : base(incoming, size)
    {
    }

    protected override long IncomingStreamPosition
    {
        get => IncomingStream.Position;
        set => IncomingStream.Position = value;
    }
}