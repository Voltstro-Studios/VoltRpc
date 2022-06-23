using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Spectre.Console;
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
        
        client.SetProtocolVersion("Demo-Protocol-1");
        
        //Add VoltRpc.Extension.Vectors
        client.TypeReaderWriterManager.InstallVectorsExtension();

        client.TypeReaderWriterManager.AddType(new CustomTypeReaderWriter());
        client.TypeReaderWriterManager.AddType(new CustomTypeArraysReaderWriter());
        
        client.AddService(typeof(ITest));

        try
        {
            client.Connect();
        }
        catch (TimeoutException)
        {
            AnsiConsole.MarkupLine("[red]The client failed to connect! Timeout.[/]");
            AnsiConsole.MarkupLine("Press any key to quit...");
            Console.ReadKey();
            return;
        }
        catch (ConnectionFailedException)
        {
            AnsiConsole.MarkupLine("[red]The client failed to connect for some unknown reason![/]");
            AnsiConsole.MarkupLine("Press any key to quit...");
            Console.ReadKey();
            return;
        }

        ITest proxy = new TestProxy(client);
        
        RunFunctionTest("Basic", proxy.BasicTest);
        RunFunctionTest("Parm", () => proxy.ParmTest("Hello World!", 142f));
        RunFunctionTest("Return", () => AnsiConsole.WriteLine($" - Return Result: {proxy.ReturnTest()}"));
        RunFunctionTest("Array", () => proxy.ArrayTest(new[] {"Hello Word!", "Bruh!"}));
        
        RunFunctionTest("Ref", () =>
        {
            string value = "Hello World!";
            AnsiConsole.WriteLine($" - Ref Before: {value}");
            proxy.RefTest(ref value);
            AnsiConsole.WriteLine($" - Ref After: {value}");
        });
        RunFunctionTest("Out", () =>
        {
            proxy.OutTest(out string message);
            AnsiConsole.WriteLine($"- Out: {message}");
        });
        
        RunFunctionTest("Guid", () =>  proxy.GuidTest(Guid.NewGuid()));
        
        RunFunctionTest("Custom Type", () => proxy.CustomTypeTest(new CustomType
        {
            Floaty = 666.6f,
            Message = "Message of the day."
        }));
        RunFunctionTest("Custom Type Return", () =>
        {
            CustomType customType = proxy.CustomTypeReturnTest();
            
            Table table = new();
            table.AddColumn("Floaty");
            table.AddColumn("Message");

            table.AddRow(customType.Floaty.ToString(CultureInfo.InvariantCulture), customType.Message);

            AnsiConsole.Write(table);
        });
        
        RunFunctionTest("Custom Type Array Small", () =>
        {
            CustomTypeArrays customTypeArray = proxy.CustomTypeArraysSmall();
            AnsiConsole.WriteLine($" - Custom Type Array size: {customTypeArray.LargeArray.Length}");
        });
        
        RunFunctionTest("Vector3 Type Return", () =>
        {
            Vector3 vector3 = proxy.Vector3TypeReturnTest();
            
            AnsiConsole.WriteLine($" - Got Vector3: {vector3}");
        });
        
        AnsiConsole.WriteLine("Press any key to quit...");
        Console.ReadKey();

        client.Dispose();
    }

    private static void RunFunctionTest(string testName, Action action)
    {
        Rule rule = new()
        {
            Alignment = Justify.Left
        };
        
        for (int i = 0; i < TestCount; i++)
        {
            rule.Title = $"Test {testName} [[{i + 1}]]";
            AnsiConsole.Write(rule);
            
            Stopwatch sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            
            rule.Title = $"Completed test in {sw.Elapsed.TotalMilliseconds}";
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }
    }
}