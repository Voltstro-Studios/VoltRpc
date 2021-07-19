using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IOTests
{
    public class MemoryStreamBufferedWriter : BufferedWriter
    {
        protected override long IncomingStreamPosition
        {
            get => OutputStream.Position;
            set => OutputStream.Position = value;
        }
        
        internal MemoryStreamBufferedWriter(MemoryStream output) : base(output)
        {
        }
    }
}