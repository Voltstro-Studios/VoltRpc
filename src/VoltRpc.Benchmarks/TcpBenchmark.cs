using System;
using System.Net;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Benchmarks;

[VoltRpcConfig]
public class TcpBenchmark : VoltRpcBenchmark
{
    private const int MinPort = 7000;
    private const int MaxPort = 8000;

    public TcpBenchmark()
    {
        Random random = new();
        int port = random.Next(MinPort, MaxPort);
        
        ConfigureClientAndHost(new TCPClient(new IPEndPoint(IPAddress.Loopback, port), bufferSize: BufferSize),
            new TCPHost(new IPEndPoint(IPAddress.Loopback, port), BufferSize));
    }
}