using System;
using System.Diagnostics;
using System.Net;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Demo.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TCPClient client = new TCPClient(new IPEndPoint(IPAddress.Loopback, 7678));
            client.AddService<ITest>();
            client.Connect();

            ITest proxy = new ITest_GeneratedProxy(client);

            //Basic test
            Console.WriteLine("Basic test #1");
            BasicTest(proxy);
            
            Console.WriteLine("Basic test #2");
            BasicTest(proxy);
            
            //Parm test
            Console.WriteLine("Parm test #1");
            ParmTest(proxy);
            
            Console.WriteLine("Parm test #2");
            ParmTest(proxy);
            
            //Return test
            Console.WriteLine("Return test #1");
            ReturnTest(proxy);
            
            Console.WriteLine("Return test #2");
            ReturnTest(proxy);
            
            //Array test
            Console.WriteLine("Array test #1");
            ArrayTest(proxy);
            
            Console.WriteLine("Array test #2");
            ArrayTest(proxy);

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }

        private static void BasicTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();
            proxy.BasicTest();
            sw.Stop();
            Console.WriteLine($"Basic test took: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void ParmTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();
            proxy.ParmTest("Hello World!", 142f);
            sw.Stop();
            Console.WriteLine($"Parm test took: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void ReturnTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine($"Got response: {proxy.ReturnTest()}");
            sw.Stop();
            Console.WriteLine($"Return test took: {sw.ElapsedMilliseconds}ms");
        }

        private static void ArrayTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();

            proxy.ArrayTest(new[] {"Hello Word!", "Bruh!"});
            sw.Stop();
            Console.WriteLine($"Array test took: {sw.ElapsedMilliseconds}ms");
        }
    }
}