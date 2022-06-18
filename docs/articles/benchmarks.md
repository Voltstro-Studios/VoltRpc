# Benchmarks

Benchmarks of VoltRpc.

All benchmarks were performed with this configuration:
``` ini
BenchmarkDotNet=v0.13.1, OS=ubuntu 21.10
Intel Core i5-10600KF CPU 4.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.100
  [Host]     : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  Job-YXCTJF : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Jit=Default  Platform=AnyCpu  
```

> [!NOTE]
> Performance may vary depending on the system, as well what .NET version you use. <br>
> If you want to test the performance your self, you can build the VoltRpc.Benchmarks project in RELEASE and run it.

## Pipes Benchmark

[![Pipes Benchmark](~/assets/images/Benchmarks/PipesBenchmark.png)](~/assets/images/Benchmarks/PipesBenchmark.png)

|               Method | array | arraySize |      message |     Mean |     Error |    StdDev |
|--------------------- |------ |---------- |------------- |---------:|----------:|----------:|
|            **BasicVoid** |     **?** |         **?** |            **?** | **8.540 μs** | **0.1634 μs** | **0.2125 μs** |
|          BasicReturn |     ? |         ? |            ? | 9.488 μs | 0.1218 μs | 0.1080 μs |
|          ArrayReturn |     ? |         ? |            ? | 9.528 μs | 0.1653 μs | 0.1546 μs |
|   **ArrayParameterVoid** |     **?** |        **25** |            **?** | **9.233 μs** | **0.0999 μs** | **0.0885 μs** |
| ArrayParameterReturn |     ? |        25 |            ? | 9.427 μs | 0.1529 μs | 0.1636 μs |
|   **ArrayParameterVoid** |     **?** |   **8294400** |            **?** | **9.290 μs** | **0.1106 μs** | **0.0924 μs** |
| ArrayParameterReturn |     ? |   8294400 |            ? | 9.631 μs | 0.1637 μs | 0.1531 μs |
|   **BasicParameterVoid** |     **?** |         **?** | **Hello World!** | **9.358 μs** | **0.1194 μs** | **0.1116 μs** |
| BasicParameterReturn |     ? |         ? | Hello World! | 9.891 μs | 0.1478 μs | 0.1383 μs |


## TCP Benchmark

[![TCP Benchmark](~/assets/images/Benchmarks/TCPBenchmark.png)](~/assets/images/Benchmarks/TCPBenchmark.png)

|               Method | array | arraySize |      message |     Mean |    Error |   StdDev |
|--------------------- |------ |---------- |------------- |---------:|---------:|---------:|
|            **BasicVoid** |     **?** |         **?** |            **?** | **23.72 μs** | **0.355 μs** | **0.332 μs** |
|          BasicReturn |     ? |         ? |            ? | 25.02 μs | 0.421 μs | 0.394 μs |
|          ArrayReturn |     ? |         ? |            ? | 25.28 μs | 0.322 μs | 0.301 μs |
|   **ArrayParameterVoid** |     **?** |        **25** |            **?** | **25.20 μs** | **0.275 μs** | **0.257 μs** |
| ArrayParameterReturn |     ? |        25 |            ? | 25.76 μs | 0.370 μs | 0.328 μs |
|   **ArrayParameterVoid** |     **?** |   **8294400** |            **?** | **24.93 μs** | **0.236 μs** | **0.220 μs** |
| ArrayParameterReturn |     ? |   8294400 |            ? | 25.22 μs | 0.468 μs | 0.438 μs |
|   **BasicParameterVoid** |     **?** |         **?** | **Hello World!** | **23.53 μs** | **0.432 μs** | **0.404 μs** |
| BasicParameterReturn |     ? |         ? | Hello World! | 25.08 μs | 0.496 μs | 0.944 μs |
