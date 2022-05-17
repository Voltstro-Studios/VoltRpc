using System;
using NUnit.Framework;
using VoltRpc.Extension.Memory;
using VoltRpc.IO;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Extensions.Memory;

public class BufferedWriterMemoryExtensionsTests
{
    [Test]
    public void SpanTest()
    {
        using DualBuffers buffers = new();

        Span<byte> data = new byte[] { 1, 2, 3, 4 };
        buffers.BufferedWriter.WriteBytesSpan(data);

        Assert.AreEqual(data.Length, buffers.BufferedWriter.Position);
        CheckBufferData(buffers.BufferedWriter, data);
    }

    [Test]
    public void SpanResizeTest()
    {
        using DualBuffers buffers = new(4);

        Span<byte> firstDataSet = new byte[] { 1, 2, 3, 4 };
        buffers.BufferedWriter.WriteBytesSpan(firstDataSet);
        
        Assert.AreEqual(firstDataSet.Length, buffers.BufferedWriter.Length);
        Assert.AreEqual(firstDataSet.Length, buffers.BufferedWriter.Position);
        CheckBufferData(buffers.BufferedWriter, firstDataSet);

        Span<byte> secondDataSet = new byte[] { 5, 6, 7, 8 };
        buffers.BufferedWriter.WriteBytesSpan(secondDataSet);

        int totalSize = firstDataSet.Length + secondDataSet.Length;
        Span<byte> result = new byte[totalSize];
        firstDataSet.CopyTo(result);
        secondDataSet.CopyTo(result);
        
        Assert.AreEqual(totalSize, buffers.BufferedWriter.Length);
        Assert.AreEqual(totalSize, buffers.BufferedWriter.Position);
        CheckBufferData(buffers.BufferedWriter, result);
    }

    [Test]
    public void MemoryTest()
    {
        using DualBuffers buffers = new();

        Memory<byte> data = new byte[] { 1, 2, 3, 4 };
        buffers.BufferedWriter.WriteBytesMemory(data);
        
        Assert.AreEqual(data.Length, buffers.BufferedWriter.Position);
        CheckBufferData(buffers.BufferedWriter, data.Span);
    }

    private void CheckBufferData(BufferedWriter writer, Span<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            Assert.AreEqual(writer.buffer[i], data[i]);
        }
    }
}