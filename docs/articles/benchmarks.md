# Benchmarks

Benchmarks of VoltRpc.

All benchmarks were performed with this configuration:
``` ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1110 (21H1/May2021Update)
Intel Core i5-10600KF CPU 4.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.302
  [Host]     : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  Job-GCFSJA : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT

Jit=Default  Platform=AnyCpu  
```

> [!NOTE]
> Performance may vary depending on the system, as well what .NET version you use. <br>
> If you want to test the performance your self, you can build the VoltRpc.Benchmarks project in RELEASE and run it.

## Pipes Benchmark

[![Pipes Benchmark](~/images/benchmarks/PipesBenchmark.png)](~/images/benchmarks/PipesBenchmark.png)

|               Method | array         |      message |     Mean |     Error |    StdDev |
|--------------------- |-------------- |------------- |---------:|----------:|----------:|
|            BasicVoid |     ?         |            ? | 7.093 μs | 0.0721 μs | 0.0602 μs |
|          BasicReturn |     ?         |            ? | 7.569 μs | 0.0902 μs | 0.0753 μs |
|   BasicParameterVoid |     ?         | Hello World! | 7.567 μs | 0.0560 μs | 0.0496 μs |
| BasicParameterReturn |     ?         | Hello World! | 8.082 μs | 0.1501 μs | 0.1787 μs |
|          ArrayReturn |     ?         |            ? | 7.473 μs | 0.0374 μs | 0.0312 μs |
|   ArrayParameterVoid |Byte[50]       |            ? | 7.604 μs | 0.0270 μs | 0.0240 μs |
|   ArrayParameterVoid |Byte[8,294,400]|            ? | 7.523 μs | 0.0901 μs | 0.0752 μs |
| ArrayParameterReturn |Byte[50]       |            ? | 7.729 μs | 0.0751 μs | 0.0666 μs |
| ArrayParameterReturn |Byte[8,294,400]|            ? | 7.852 μs | 0.0625 μs | 0.0585 μs |

## TCP Benchmark

[![TCP Benchmark](~/images/benchmarks/TCPBenchmark.png)](~/images/benchmarks/TCPBenchmark.png)

|               Method | array         |      message |     Mean |     Error |    StdDev |
|--------------------- |-------------- |------------- |---------:|----------:|----------:|
|            BasicVoid |     ?         |            ? | 12.01 μs | 0.072 μs  | 0.067 μs  |
|          BasicReturn |     ?         |            ? | 12.61 μs | 0.065 μs  | 0.061 μs  |
|   BasicParameterVoid |     ?         | Hello World! | 12.35 μs | 0.053 μs  | 0.050 μs  |
| BasicParameterReturn |     ?         | Hello World! | 12.39 μs | 0.066 μs  | 0.062 μs  |
|          ArrayReturn |     ?         |            ? | 12.54 μs | 0.048 μs  | 0.040 μs  |
|   ArrayParameterVoid |Byte[50]       |            ? | 12.61 μs | 0.062 μs  | 0.058 μs  |
|   ArrayParameterVoid |Byte[8,294,400]|            ? | 12.62 μs | 0.087 μs  | 0.082 μs  |
| ArrayParameterReturn |Byte[50]       |            ? | 13.30 μs | 0.265 μs  | 0.248 μs  |
| ArrayParameterReturn |Byte[8,294,400]|            ? | 13.44 μs | 0.084 μs  | 0.079 μs  |