using System;
using VoltRpc.Demo.Shared;
using VoltRpc.Logging;
using VoltRpc.Communication.Pipes;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Demo.Host
{
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
                host = new TCPHost(parser.IpEndPoint, logger, Communication.Host.DefaultBufferSize, TCPHost.DefaultSendTimeout);

            TestImp testImp = new();
            host.AddService<ITest>(testImp);
            host.StartListening();
            
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            host.Dispose();
        }
    }
}