using System;
using System.Diagnostics;
using System.Net;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;

namespace VoltRpc.Demo.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TCPClient client = new TCPClient(new IPEndPoint(IPAddress.Loopback, 7678));
            client.AddService<ITest>();
            client.Connect();
            
            //Basic test
            Console.WriteLine("Basic test #1");
            BasicTest(client);
            
            Console.WriteLine("Basic test #2");
            BasicTest(client);
            
            //Parm test
            Console.WriteLine("Parm test #1");
            ParmTest(client);
            
            Console.WriteLine("Parm test #2");
            ParmTest(client);
            
            //Return test
            Console.WriteLine("Return test #1");
            ReturnTest(client);
            
            Console.WriteLine("Return test #2");
            ReturnTest(client);

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }

        private static void BasicTest(Communication.Client client)
        {
            Stopwatch sw = Stopwatch.StartNew();
            client.InvokeMethod("VoltRpc.Demo.Shared.ITest.BasicTest");
            sw.Stop();
            Console.WriteLine($"Basic test took: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void ParmTest(Communication.Client client)
        {
            Stopwatch sw = Stopwatch.StartNew();
            client.InvokeMethod("VoltRpc.Demo.Shared.ITest.ParmTest", (string)null);
            sw.Stop();
            Console.WriteLine($"Parm test took: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void ReturnTest(Communication.Client client)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Got response: {(string)client.InvokeMethod("VoltRpc.Demo.Shared.ITest.ReturnTest")}");
            sw.Stop();
            Console.WriteLine($"Return test took: {sw.ElapsedMilliseconds}ms");
        }
    }
}