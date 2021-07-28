using System;
using System.Linq;
using System.Net;

namespace VoltRpc.Demo.Shared
{
    public class ArgsParser
    {
        private const int DefaultPort = 8877;

        public IPEndPoint IpEndPoint = new(IPAddress.Loopback, DefaultPort);

        public string PipeName = "VoltRpcPipe";

        public bool PipesClient;

        public void ParseArgs(string[] args)
        {
            if (args.Contains("-pipes"))
                PipesClient = true;
            if (args.Contains("-tcp"))
                PipesClient = false;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (i + 1! < args.Length)
                    switch (arg)
                    {
                        case "-ip":
                        {
                            string possibleIp = args[i + 1];
                            IpEndPoint.Address = IPAddress.Parse(possibleIp);
                            break;
                        }
                        case "-port":
                        {
                            string possiblePort = args[i + 1];
                            IpEndPoint.Port = int.Parse(possiblePort);
                            break;
                        }
                        case "-name":
                        {
                            string pipeName = args[i + 1];
                            if (string.IsNullOrWhiteSpace(pipeName))
                                throw new ArgumentNullException("-name", "The pipe name is empty or null!");
                            PipeName = pipeName;
                            break;
                        }
                    }
            }
        }
    }
}