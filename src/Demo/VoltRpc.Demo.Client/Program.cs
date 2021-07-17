using System;
using System.Net;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Demo.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TCPClient client = new TCPClient(new IPEndPoint(IPAddress.Loopback, 7678));
            client.Connect();
            
            client.InvokeMethod("VoltRpc.Demo.Shared.ITest.BasicTest");
            client.InvokeMethod("VoltRpc.Demo.Shared.ITest.ParmTest", "Hello World!");

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }
    }
}