using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;

namespace VoltRpc.Tests.Types;

public static class Utils
{
    public static void TestTypeReaderWriter<T>(ITypeReadWriter readWriter, T value)
    {
        using DualBuffers buffers = new();
        readWriter.Write(buffers.BufferedWriter, value);
        buffers.BufferedWriter.Flush();

        T result = (T) readWriter.Read(buffers.BufferedReader);
        Assert.AreEqual(value, result);
    }
}