using VoltRpc.Proxy;

namespace VoltRpc.Demo.Shared
{
    [GenerateProxy]
    public interface ITest
    {
        public void BasicTest();

        public void ParmTest(string message, float num);

        public string ReturnTest();
        
        public void ArrayTest(string[] array);
    }
}