using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;

namespace VoltRpc.Tests.Types;

public static class Utils
{
    public static void TestTypeReaderWriter<T>(TypeReadWriter<T> readWriter, T value)
    {
        using DualBuffers buffers = new();
        readWriter.Write(buffers.BufferedWriter, value);
        buffers.BufferedWriter.Flush();

        T result = readWriter.Read(buffers.BufferedReader);
        Assert.AreEqual(value, result);
    }
}