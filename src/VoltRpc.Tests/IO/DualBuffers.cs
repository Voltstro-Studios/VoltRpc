using System;
using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO
{
    public class DualBuffers : IDisposable
    {
        public DualBuffers()
        {
            memoryStream = new MemoryStream(1000);
            BufferedReader = new MemoryStreamBufferedReader(memoryStream);
            BufferedWriter = new MemoryStreamBufferedWriter(memoryStream);
        }
        
        private readonly MemoryStream memoryStream;

        public BufferedReader BufferedReader { get; }
        public BufferedWriter BufferedWriter { get; }

        ~DualBuffers()
        {
            ReleaseResources();
        }
        
        public void Dispose()
        {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseResources()
        {
            BufferedReader?.Dispose();
            BufferedWriter?.Dispose();
            memoryStream?.Dispose();
        }
    }
}