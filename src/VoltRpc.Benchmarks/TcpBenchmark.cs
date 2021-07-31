using System.Net;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.TCP;
using VoltRpc.Logging;

namespace VoltRpc.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    public class TcpBenchmark : VoltRpcBenchmark
    {
        private const int Port = 7678;

        public TcpBenchmark()
            : base(new TCPClient(new IPEndPoint(IPAddress.Loopback, Port), 7000), 
                new TCPHost(new IPEndPoint(IPAddress.Loopback, Port), new NullLogger(), TCPHost.DefaultReceiveTimeout, TCPHost.DefaultSendTimeout))
        {
        }
    }
}
