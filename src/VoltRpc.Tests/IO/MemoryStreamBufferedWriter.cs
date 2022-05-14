using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class MemoryStreamBufferedWriter : BufferedWriter
{
    internal MemoryStreamBufferedWriter(MemoryStream output, int size) 
        : base(output, size)
    {
    }

    protected override long OutputStreamPosition
    {
        get => OutputStream.Position;
        set => OutputStream.Position = value;
    }
}