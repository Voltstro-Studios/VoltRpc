namespace VoltRpc.Demo.Shared
{
    public interface ITest
    {
        public void BasicTest();

        public void ParmTest(string message, float num);

        public string ReturnTest();
        
        public void ArrayTest(string[] array);
    }
}