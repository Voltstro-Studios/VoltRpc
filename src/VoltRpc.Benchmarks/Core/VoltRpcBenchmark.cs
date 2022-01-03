using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using VoltRpc.Benchmarks.Interface;
using VoltRpc.Communication;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Benchmarks.Core;

public abstract class VoltRpcBenchmark
{
    private const int smallArraySize = 25;
    private const int largeArraySize = 1920 * 1080 * 4;
    private readonly Client client;
    private readonly Host host;

    private IBenchmarkInterface benchmarkProxy;
    private byte[] bigArray;

    private byte[] smallArray;

    protected VoltRpcBenchmark(Client client, Host host)
    {
        this.client = client;
        this.host = host;
    }

    [GlobalSetup]
    public void Setup()
    {
        host.AddService<IBenchmarkInterface>(new BenchmarkInterfaceImpl());
        host.StartListening();
        host.MaxConnectionsCount = 1;

        client.AddService<IBenchmarkInterface>();
        client.Connect();
        benchmarkProxy = new BenchmarkProxy(client);

        smallArray = new byte[smallArraySize];
        smallArray = Utils.FillByteArray(smallArray);
        bigArray = new byte[largeArraySize];
        bigArray = Utils.FillByteArray(bigArray);
    }

    [Benchmark]
    public void BasicVoid()
    {
        benchmarkProxy.BasicVoid();
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetMessage))]
    public void BasicParameterVoid(string message)
    {
        benchmarkProxy.BasicParameterVoid(message);
    }

    [Benchmark]
    public string BasicReturn()
    {
        return benchmarkProxy.BasicReturn();
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetMessage))]
    public string BasicParameterReturn(string message)
    {
        return benchmarkProxy.BasicParameterReturn(message);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetArray))]
    public void ArrayParameterVoid(byte[] array, int arraySize)
    {
        benchmarkProxy.ArrayParameterVoid(array);
    }

    [Benchmark]
    public byte[] ArrayReturn()
    {
        return benchmarkProxy.ArrayReturn();
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetArray))]
    public byte[] ArrayParameterReturn(byte[] array, int arraySize)
    {
        return benchmarkProxy.ArrayParameterReturn(array);
    }

    public IEnumerable<object[]> GetArray()
    {
        yield return new object[] {smallArray, smallArraySize};
        yield return new object[] {bigArray, largeArraySize};
    }

    public IEnumerable<string> GetMessage()
    {
        yield return "Hello World!";
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        try
        {
            client.Dispose();
            host.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }
    }
}