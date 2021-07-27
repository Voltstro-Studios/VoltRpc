using System;
using System.Diagnostics;
using VoltRpc.Demo.Shared;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Demo.Client
{
    public static class Program
    {
        private const int TestCount = 3;

        public static void Main(string[] args)
        {
            ArgsParser parser = new();
            parser.ParseArgs(args);

            Communication.Client client;
            if (parser.PipesClient)
                client = new PipesClient(parser.PipeName);
            else
                client = new TCPClient(parser.IpEndPoint);

            client.AddService<ITest>();
            client.Connect();

            ITest proxy = new ITest_GeneratedProxy(client);

            RunFunctionTest("Basic", proxy.BasicTest);
            RunFunctionTest("Parm", () => proxy.ParmTest("Hello World!", 142f));
            RunFunctionTest("Return", () => Console.WriteLine($"Got Return: {proxy.ReturnTest()}"));
            RunFunctionTest("Array", () => proxy.ArrayTest(new[] {"Hello Word!", "Bruh!"}));
            RunFunctionTest("Ref", () =>
            {
                string value = "Hello World!";
                proxy.RefTest(ref value);
                Console.WriteLine($"Got ref value back as: {value}");
            });
            RunFunctionTest("Out", () =>
            {
                proxy.OutTest(out string message);
                Console.WriteLine($"Got out as: {message}");
            });
            
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }

        private static void RunFunctionTest(string testName, Action action)
        {
            for (int i = 0; i < TestCount; i++)
            {
                Console.WriteLine($"Running test {testName} #{i + 1}...");
                Stopwatch sw = Stopwatch.StartNew();
                action();
                sw.Stop();
                Console.WriteLine($"{testName} test #{i + 1} took: {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}