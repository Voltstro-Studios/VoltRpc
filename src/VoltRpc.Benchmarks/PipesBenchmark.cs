using System;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.Pipes;

namespace VoltRpc.Benchmarks;

[VoltRpcConfig]
public class PipesBenchmark : VoltRpcBenchmark
{
    public PipesBenchmark()
    {
        Guid guid = Guid.NewGuid();
        string pipeName = guid.ToString("N");
        ConfigureClientAndHost(new PipesClient(pipeName), new PipesHost(pipeName));
    }
}