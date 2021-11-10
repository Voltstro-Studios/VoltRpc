using System;
using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Tests.IO;

public class DualBuffers : IDisposable
{
    private readonly MemoryStream memoryStream;

    public DualBuffers()
    {
        memoryStream = new MemoryStream(1000);
        BufferedReader = new MemoryStreamBufferedReader(memoryStream);
        BufferedWriter = new MemoryStreamBufferedWriter(memoryStream);
    }

    public BufferedReader BufferedReader { get; }
    public BufferedWriter BufferedWriter { get; }

    public void Dispose()
    {
        ReleaseResources();
        GC.SuppressFinalize(this);
    }

    ~DualBuffers()
    {
        ReleaseResources();
    }

    private void ReleaseResources()
    {
        BufferedReader?.Dispose();
        BufferedWriter?.Dispose();
        memoryStream?.Dispose();
    }
}