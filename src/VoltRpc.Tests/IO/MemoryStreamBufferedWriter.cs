using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class MemoryStreamBufferedWriter : BufferedWriter
{
    internal MemoryStreamBufferedWriter(MemoryStream output) : base(output)
    {
    }

    protected override long OutputStreamPosition
    {
        get => OutputStream.Position;
        set => OutputStream.Position = value;
    }
}