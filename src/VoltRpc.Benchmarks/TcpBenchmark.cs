using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Benchmarks
{
    public class TcpBenchmark : VoltRpcBenchmark
    {
        private const int Port = 7678;

        public TcpBenchmark()
            : base(new TCPClient(new IPEndPoint(IPAddress.Loopback, Port)), 
                new TCPHost(new IPEndPoint(IPAddress.Loopback, Port)))
        {
        }
    }
}
