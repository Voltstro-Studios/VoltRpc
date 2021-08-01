namespace VoltRpc.Benchmarks.Interface
{
    public class BenchmarkInterfaceImpl : IBenchmarkInterface
    {
        private readonly byte[] smallArray;

        public BenchmarkInterfaceImpl()
        {
            smallArray = new byte[25];
            smallArray = Utils.FillByteArray(smallArray);
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
            return array;
        }
    }
}