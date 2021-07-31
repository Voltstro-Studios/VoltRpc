using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.Pipes;

namespace VoltRpc.Benchmarks
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net50)]
    public class PipesBenchmark : VoltRpcBenchmark
    {
        private const string PipesName = "BenchmarkPipe";

        public PipesBenchmark()
            : base(new PipesClient(PipesName), new PipesHost(PipesName, 128))
        {
        }
    }
}