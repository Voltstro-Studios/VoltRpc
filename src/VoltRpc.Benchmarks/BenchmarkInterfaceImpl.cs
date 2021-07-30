using System;

namespace VoltRpc.Benchmarks
{
    public class BenchmarkInterfaceImpl : IBenchmarkInterface
    {
        private readonly byte[] smallArray;

        private readonly byte[] bigArray;
        
        public BenchmarkInterfaceImpl()
        {
            smallArray = new byte[25];
            smallArray = Utils.FillByteArray(smallArray);

            bigArray = new byte[1920 * 1080 * 4];
            bigArray = Utils.FillByteArray(bigArray);
        }
        
        public void BasicVoid()
        {
        }

        public void BasicParameterVoid(string message)
        {
        }

        public string BasicReturn()
        {
            return "Hello World!";
        }

        public string BasicParameterReturn(string message)
        {
            return "Hello World!";
        }

        public void ArrayParameterVoid(byte[] array)
        {
        }

        public byte[] ArrayReturn()
        {
            return smallArray;
        }

        public byte[] ArrayParameterReturn(byte[] array)
        {
            return smallArray;
        }

        public void BigAssArrayParameterVoid(byte[] array)
        {
        }

        public byte[] BigAssArrayReturn()
        {
            return bigArray;
        }

        public byte[] BigAssArrayParameterReturn(byte[] array)
        {
            return bigArray;
        }
    }
}