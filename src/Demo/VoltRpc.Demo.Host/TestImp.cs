using System;
using VoltRpc.Demo.Shared;

namespace VoltRpc.Demo.Host
{
    public class TestImp : ITest
    {
        public void BasicTest()
        {
            Console.WriteLine("Basic Test!");
        }

        public void ParmTest(string message)
        {
            Console.WriteLine(message);
        }
    }
}