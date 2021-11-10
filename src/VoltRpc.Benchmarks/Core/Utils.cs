using System;

namespace VoltRpc.Benchmarks
{
    public static class Utils
    {
        public static byte[] FillByteArray(byte[] array)
        {
            Random random = new Random();
            for (int i = 0; i < array.Length; i++) array[i] = (byte) random.Next(byte.MinValue, byte.MinValue);

            return array;
        }
    }
}