using System;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Logging;

namespace VoltRpc.Demo.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        ArgsParser parser = new();
        parser.ParseArgs(args);

        ILogger logger = new ConsoleLogger(LogVerbosity.Debug);

        Communication.Host host;
        if (parser.PipesClient)
            host = new PipesHost(parser.PipeName, logger);
        else
            host = new TCPHost(parser.IpEndPoint, logger);
        
        host.SetProtocolVersion("Demo-Protocol-1");
        
        //Add VoltRpc.Extension.Vectors
        host.TypeReaderWriterManager.InstallVectorsExtension();
        
        host.TypeReaderWriterManager.AddType(new CustomTypeReaderWriter());
        host.TypeReaderWriterManager.AddType(new CustomTypeArraysReaderWriter());
        
        host.MaxConnectionsCount = 1;

        TestImp testImp = new();
        host.AddService(typeof(ITest), testImp);
        host.StartListeningAsync().ConfigureAwait(false);

        Console.WriteLine("Press any key to quit...");
        Console.ReadKey();
        host.Dispose();
    }
}