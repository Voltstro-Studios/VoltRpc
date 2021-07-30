using System.Net;
using BenchmarkDotNet.Attributes;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;
using VoltRpc.Logging;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Benchmarks
{
    public class VoltRpcBenchmark
    {
        private readonly Host host;
        private readonly Client client;

        private readonly IBenchmarkInterface benchmarkProxy;

        private readonly byte[] smallArray;
        
        public VoltRpcBenchmark()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 7788);
            host = new TCPHost(ipEndPoint, new NullLogger(), 
                Host.DefaultBufferSize, 
                6000000);
            host.AddService<IBenchmarkInterface>(new BenchmarkInterfaceImpl());
            host.StartListening();

            client = new TCPClient(ipEndPoint, 7000);
            client.AddService<IBenchmarkInterface>();
            client.Connect();
            benchmarkProxy = new BenchmarkProxy(client);

            smallArray = new byte[25];
            smallArray = Utils.FillByteArray(smallArray);
        }

        [Benchmark]
        public void BasicVoid() => benchmarkProxy.BasicVoid();

        [Benchmark]
        public void BasicParameterVoid() => benchmarkProxy.BasicParameterVoid("Hello World!");

        [Benchmark]
        public string BasicReturn() => benchmarkProxy.BasicReturn();

        [Benchmark]
        public string BasicParameterReturn() => benchmarkProxy.BasicParameterReturn("Hello World!");

        [Benchmark]
        public void ArrayParameterVoid() => benchmarkProxy.ArrayParameterVoid(smallArray);

        [Benchmark]
        public byte[] ArrayReturn() => benchmarkProxy.ArrayReturn();

        [Benchmark]
        public byte[] ArrayParameterReturn() => benchmarkProxy.ArrayParameterReturn(smallArray);
    }
}