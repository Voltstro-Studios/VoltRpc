using System;
using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class DualBuffers : IDisposable
{
    private readonly MemoryStream memoryStream;

    public bool HasDisposed { get; private set; }

    public DualBuffers(int size = 1000)
    {
        memoryStream = new MemoryStream(size);
        BufferedReader = new MemoryStreamBufferedReader(memoryStream, size);
        BufferedWriter = new MemoryStreamBufferedWriter(memoryStream, size);
    }

    public MemoryStreamBufferedReader BufferedReader { get; }
    public MemoryStreamBufferedWriter BufferedWriter { get; }

    public void Flush()
    {
        memoryStream.Position = 0;
        BufferedReader.Position = 0;
        BufferedReader.readLength = 0;

        BufferedWriter.Flush();
    }

    public void Dispose()
    {
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseResources()
    {
        if(HasDisposed)
            return;
        
        BufferedReader?.Dispose();
        BufferedWriter?.Dispose();
        memoryStream?.Dispose();
        HasDisposed = true;
    }
}