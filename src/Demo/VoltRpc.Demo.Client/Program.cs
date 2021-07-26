using System;
using System.Diagnostics;
using System.Net;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Communication.Pipes;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Demo.Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Communication.Client client = new PipesClient("TestPipe");
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
            
            //Ref test
            Console.WriteLine("Ref test #1");
            RefTest(proxy);
            
            Console.WriteLine("Ref test #2");
            RefTest(proxy);
            
            //Out test
            Console.WriteLine("Out test #1");
            OutTest(proxy);
            
            Console.WriteLine("Out test #2");
            OutTest(proxy);

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

        private static void RefTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string value = "Hello World!";
            proxy.RefTest(ref value);
            Console.WriteLine($"Ref value is: {value}");
            sw.Stop();
            Console.WriteLine($"Ref test took: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void OutTest(ITest proxy)
        {
            Stopwatch sw = Stopwatch.StartNew();
            proxy.OutTest(out string message);
            Console.WriteLine($"Out value is: {message}");
            sw.Stop();
            Console.WriteLine($"Out test took: {sw.ElapsedMilliseconds}ms");
        }
    }
}