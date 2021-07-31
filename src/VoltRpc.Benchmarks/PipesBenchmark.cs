using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using VoltRpc.Benchmarks.Core;
using VoltRpc.Communication.Pipes;

namespace VoltRpc.Benchmarks
{
    public class PipesBenchmark : VoltRpcBenchmark
    {
        private const string PipesName = "BenchmarkPipe";

        public PipesBenchmark()
            : base(new PipesClient(PipesName), new PipesHost(PipesName))
        {
        }
    }
}