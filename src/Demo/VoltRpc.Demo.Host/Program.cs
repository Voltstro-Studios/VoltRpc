using System;
using System.Net;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Demo.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            TCPHost host = new TCPHost(new IPEndPoint(IPAddress.Loopback, 7678));
            host.StartListening();
            
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            host.Dispose();
        }
    }
}