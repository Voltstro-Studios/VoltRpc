using System;

namespace VoltRpc.Benchmarks.Core;

public static class Utils
{
    public static byte[] FillByteArray(byte[] array)
    {
        Random random = new();
        for (int i = 0; i < array.Length; i++) array[i] = (byte) random.Next(byte.MinValue, byte.MinValue);

        return array;
    }
}