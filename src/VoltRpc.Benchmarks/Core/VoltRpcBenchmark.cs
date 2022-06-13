using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using VoltRpc.Benchmarks.Core.FastArray;
using VoltRpc.Benchmarks.Interface;
using VoltRpc.Communication;
using VoltRpc.Proxy.Generated;

namespace VoltRpc.Benchmarks.Core;

public abstract class VoltRpcBenchmark
{
    protected const int BufferSize = 8294500;
    
    private const int SmallArraySize = 25;
    private const int LargeArraySize = 1920 * 1080 * 4;
    
    private Client client;
    private Host host;

    private IBenchmarkInterface benchmarkProxy;
    
    private static byte[] bigArray;
    private static byte[] smallArray;

    protected void ConfigureClientAndHost(Client configuredClient, Host configuredHost)
    {
        Random random = Random.Shared;
        smallArray = new byte[SmallArraySize];
        random.NextBytes(smallArray);

        bigArray = new byte[LargeArraySize];
        random.NextBytes(bigArray);
        
        client = configuredClient;
        host = configuredHost;
    }

    [GlobalSetup]
    public void Setup()
    {
        host.TypeReaderWriterManager.AddType(new FastArrayTypeReaderWriter());
        host.AddService<IBenchmarkInterface>(new BenchmarkInterfaceImpl(smallArray, bigArray));
        host.StartListeningAsync().ConfigureAwait(false);
        host.MaxConnectionsCount = 1;

        client.TypeReaderWriterManager.AddType(new FastArrayTypeReaderWriter());
        client.AddService<IBenchmarkInterface>();
        client.Connect();
        benchmarkProxy = new BenchmarkProxy(client);
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
    public void ArrayParameterVoid(byte[] array)
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
    public byte[] ArrayParameterReturn(byte[] array)
    {
        return benchmarkProxy.ArrayParameterReturn(array);
    }

    [Benchmark]
    public void ArrayFast()
    {
        benchmarkProxy.ArrayFast();
    }

    public static IEnumerable<object> GetArray()
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