using System;
using Spectre.Console;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;
using VoltRpc.Demo.Shared;
using VoltRpc.Logging;

namespace VoltRpc.Demo.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        ArgsParser parser = new();
        parser.ParseArgs(args);

        bool errorOccured = false;

        AnsiConsole.Status().Start("Press any key to quit...", _ =>
        {
            try
            {
                ILogger logger = new SpectreLogger(LogVerbosity.Debug);

                Communication.Host host;
                if (parser.PipesClient)
                    host = new PipesHost(parser.PipeName, logger);
                else
                    host = new TCPHost(parser.IpEndPoint, logger);
        
                host.SetProtocolVersion("Demo-Protocol-1");
        
                host.TypeReaderWriterManager.AddType(new CustomTypeReaderWriter());
                host.TypeReaderWriterManager.AddType(new CustomTypeArraysReaderWriter());
        
                host.MaxConnectionsCount = 1;

                TestImp testImp = new();
                host.AddService(typeof(ITest), testImp);
                host.StartListeningAsync().ConfigureAwait(false);

                Console.ReadKey();
                host.Dispose();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                errorOccured = true;
            }
        });

        if (!errorOccured) 
            return;
        
        //Some error occured
        AnsiConsole.MarkupLine("[red]An error occured while running the host! Please read the log above to see the error. Press any key to quit...[/]");
        Console.ReadKey();
    }
}