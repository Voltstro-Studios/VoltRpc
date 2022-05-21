using System;
using System.Diagnostics;
using System.Numerics;
using VoltRpc.Communication;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Demo.Client;

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
        
        //Add VoltRpc.Extension.Vectors
        client.TypeReaderWriterManager.InstallVectorsExtension();

        client.TypeReaderWriterManager.AddType(new CustomTypeReaderWriter());
        client.AddService(typeof(ITest));

        try
        {
            client.Connect();
        }
        catch (TimeoutException)
        {
            Console.WriteLine("The client failed to connect! Timeout.");
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            return;
        }
        catch (ConnectionFailedException)
        {
            Console.WriteLine("The client failed to connect for some unknown reason!");
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            return;
        }

        ITest proxy = new TestProxy(client);

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
        RunFunctionTest("Custom Type", () => proxy.CustomTypeTest(new CustomType
        {
            Floaty = 666.6f,
            Message = "Message of the day."
        }));
        RunFunctionTest("Custom Type Return", () =>
        {
            CustomType customType = proxy.CustomTypeReturnTest();
            Console.WriteLine($"Got custom type with values: {customType.Floaty} {customType.Message}.");
        });
        RunFunctionTest("Vector3 Type Return", () =>
        {
            Vector3 vector3 = proxy.Vector3TypeReturnTest();
            Console.WriteLine($"Got vector3 type with value: {vector3}.");
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
            Console.WriteLine($"{testName} test #{i + 1} took: {sw.Elapsed.TotalMilliseconds}ms");
        }
    }
}