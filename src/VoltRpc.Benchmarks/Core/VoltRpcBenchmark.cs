using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using VoltRpc.Benchmarks.Interface;
using VoltRpc.Communication;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Benchmarks.Core
{
    public abstract class VoltRpcBenchmark
    {
        private readonly Host host;
        private readonly Client client;

        private readonly IBenchmarkInterface benchmarkProxy;

        private readonly byte[] smallArray;
        private readonly byte[] bigArray;
        
        protected VoltRpcBenchmark(Client client, Host host)
        {
            this.host = host;
            host.AddService<IBenchmarkInterface>(new BenchmarkInterfaceImpl());
            host.StartListening();

            this.client = client;
            client.AddService<IBenchmarkInterface>();
            client.Connect();
            benchmarkProxy = new BenchmarkProxy(client);

            smallArray = new byte[25];
            smallArray = Utils.FillByteArray(smallArray);
            bigArray = new byte[1920 * 1080 * 4];
            bigArray = Utils.FillByteArray(bigArray);
        }

        [Benchmark]
        public void BasicVoid() => benchmarkProxy.BasicVoid();

        [Benchmark]
        [ArgumentsSource(nameof(GetMessage))]
        public void BasicParameterVoid(string message) => benchmarkProxy.BasicParameterVoid(message);

        [Benchmark]
        public string BasicReturn() => benchmarkProxy.BasicReturn();

        [Benchmark]
        [ArgumentsSource(nameof(GetMessage))]
        public string BasicParameterReturn(string message) => benchmarkProxy.BasicParameterReturn(message);

        [Benchmark]
        [ArgumentsSource(nameof(GetArray))]
        public void ArrayParameterVoid(byte[] array) => benchmarkProxy.ArrayParameterVoid(array);

        [Benchmark]
        public byte[] ArrayReturn() => benchmarkProxy.ArrayReturn();

        [Benchmark]
        [ArgumentsSource(nameof(GetArray))]
        public byte[] ArrayParameterReturn(byte[] array) => benchmarkProxy.ArrayParameterReturn(array);

        public IEnumerable<byte[]> GetArray()
        {
            yield return smallArray;
            yield return bigArray;
        }

        public IEnumerable<string> GetMessage()
        {
            yield return "Hello World!";
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            client.Dispose();
            host.Dispose();
        }
    }
}