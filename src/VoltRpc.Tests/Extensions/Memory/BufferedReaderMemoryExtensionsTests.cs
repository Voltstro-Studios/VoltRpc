using System;
using NUnit.Framework;
using VoltRpc.Extension.Memory;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Extensions.Memory;

public class BufferedReaderMemoryExtensionsTests
{
    [Test]
    public void ReadSpanSliceTest()
    {
        using DualBuffers buffers = new();
        ReadOnlySpan<byte> data = new byte[] { 1, 2, 3, 4 };
        buffers.BufferedWriter.WriteBytesSpan(data);
        buffers.BufferedWriter.Flush();

        ReadOnlySpan<byte> result = buffers.BufferedReader.ReadBytesSpanSlice(data.Length);
        CheckData(data, result);
    }

    [Test]
    public void ReadSpanCopyTest()
    {
        using DualBuffers buffers = new();
        ReadOnlySpan<byte> data = new byte[] { 1, 2, 3, 4 };
        buffers.BufferedWriter.WriteBytesSpan(data);
        buffers.BufferedWriter.Flush();

        ReadOnlySpan<byte> result = buffers.BufferedReader.ReadBytesSpanCopy(data.Length);
        CheckData(data, result);
    }
    
    private void CheckData(ReadOnlySpan<byte> expected, ReadOnlySpan<byte> data)
    {
        Assert.AreEqual(expected.Length, data.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.AreEqual(expected[i], data[i]);
        }
    }
}