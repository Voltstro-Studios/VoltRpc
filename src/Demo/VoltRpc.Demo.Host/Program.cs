using System;
using System.Net;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Logging;

namespace VoltRpc.Demo.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TestImp testImp = new TestImp();

            TCPHost host = new TCPHost(new IPEndPoint(IPAddress.Loopback, 7678), new ConsoleLogger(LogVerbosity.Debug));
            host.AddService<ITest>(testImp);
            host.StartListening();
            
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            host.Dispose();
        }
    }
}