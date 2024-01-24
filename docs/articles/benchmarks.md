# Benchmarks

Benchmarks of VoltRpc.

All benchmarks were performed with this configuration:
``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1826 (21H2)
Intel Core i5-10600KF CPU 4.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=6.0.302
  [Host]     : .NET 6.0.7 (6.0.722.32202), X64 RyuJIT
  Job-PJGDAZ : .NET 6.0.7 (6.0.722.32202), X64 RyuJIT

Jit=Default  Platform=AnyCpu  Runtime=.NET 6.0  
```

> [!NOTE]
> Performance may vary depending on the system, as well what .NET version you use. <br>
> If you want to test the performance your self, you can build the VoltRpc.Benchmarks project in RELEASE and run it.'

> [!WARNING]
> Currently, the in-built arrays are quite a lot slower, we are looking into fixing this in a later release.
> For using fast arrays, see the demo's [CustomTypeArraysReaderWriter](https://github.com/Voltstro-Studios/VoltRpc/blob/946f9216560e9fa950692ef7f24f08097079e4b8/src/Demo/VoltRpc.Demo.Shared/CustomTypeArraysReaderWriter.cs).

## Pipes Benchmark

![Pipes Benchmark Non-Array](~/assets/images/Benchmarks/PipesBenchmarkNonArray.png)

![Pipes Benchmark Array](~/assets/images/Benchmarks/PipesBenchmarkArray.png)

|               Method |      message |         array |             Mean |          Error |         StdDev |
|--------------------- |------------- |-------------- |-----------------:|---------------:|---------------:|
|            **BasicVoid** |            **?** |             **?** |         **6.311 μs** |      **0.0517 μs** |      **0.0432 μs** |
|          BasicReturn |            ? |             ? |         7.444 μs |      0.0589 μs |      0.0551 μs |
|          ArrayReturn |            ? |             ? |        21.389 μs |      0.3943 μs |      0.6695 μs |
|            ArrayFast |            ? |             ? |     1,579.432 μs |     10.1223 μs |      9.4684 μs |
|   **BasicParameterVoid** | **Hello World!** |             **?** |         **7.128 μs** |      **0.0390 μs** |      **0.0346 μs** |
| BasicParameterReturn | Hello World! |             ? |         8.287 μs |      0.0364 μs |      0.0304 μs |
|   **ArrayParameterVoid** |            **?** |      **Byte[25]** |        **18.423 μs** |      **0.3675 μs** |      **0.6140 μs** |
| ArrayParameterReturn |            ? |      Byte[25] |        28.647 μs |      0.5643 μs |      0.9112 μs |
|   **ArrayParameterVoid** |            **?** | **Byte[8294400]** | **2,705,942.687 μs** | **11,965.3253 μs** | **11,192.3727 μs** |
| ArrayParameterReturn |            ? | Byte[8294400] | 5,416,337.679 μs | 21,036.1040 μs | 18,647.9583 μs |

## TCP Benchmark

![TCP Benchmark Non-Array](~/assets/images/Benchmarks/TCPBenchmarkNonArray.png)

![TCP Benchmark Array](~/assets/images/Benchmarks/TCPBenchmarkArray.png)

|               Method |      message |         array |            Mean |         Error |        StdDev |
|--------------------- |------------- |-------------- |----------------:|--------------:|--------------:|
|            **BasicVoid** |            **?** |             **?** |        **13.69 μs** |      **0.139 μs** |      **0.124 μs** |
|          BasicReturn |            ? |             ? |        15.88 μs |      0.157 μs |      0.146 μs |
|          ArrayReturn |            ? |             ? |        28.70 μs |      0.126 μs |      0.111 μs |
|            ArrayFast |            ? |             ? |     3,213.17 μs |     35.051 μs |     32.786 μs |
|   **BasicParameterVoid** | **Hello World!** |             **?** |        **15.52 μs** |      **0.285 μs** |      **0.267 μs** |
| BasicParameterReturn | Hello World! |             ? |        17.17 μs |      0.167 μs |      0.156 μs |
|   **ArrayParameterVoid** |            **?** |      **Byte[25]** |        **26.89 μs** |      **0.107 μs** |      **0.100 μs** |
| ArrayParameterReturn |            ? |      Byte[25] |        40.42 μs |      0.426 μs |      0.398 μs |
|   **ArrayParameterVoid** |            **?** | **Byte[8294400]** | **3,689,192.76 μs** | **13,520.734 μs** | **11,985.778 μs** |
| ArrayParameterReturn |            ? | Byte[8294400] | 7,400,713.25 μs | 11,952.742 μs | 10,595.794 μs |
