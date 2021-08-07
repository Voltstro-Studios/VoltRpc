<img align="right" width="15%" src="media/icon.svg">

# VoltRpc

[![License](https://img.shields.io/github/license/Voltstro-Studios/VoltRpc)](/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/v/VoltRpc?label=NuGet)](https://www.nuget.org/packages/VoltRpc/)
[![NuGet Download Count](https://img.shields.io/nuget/dt/VoltRpc?label=Downloads&logo=nuget&color=blue&logoColor=blue)](https://www.nuget.org/packages/VoltRpc/)
[![Build Status](https://img.shields.io/azure-devops/build/Voltstro-Studios/63163ef8-da1d-42b6-b8b9-689420a730e5/9?logo=azure-pipelines)](https://dev.azure.com/Voltstro-Studios/VoltRpc/_build/latest?definitionId=9&branchName=master)
[![Docs Status](https://img.shields.io/website?down_color=red&down_message=Offline&label=Docs&up_color=blue&up_message=Online&url=https%3A%2F%2Fvoltrpc.voltstro.dev)](https://voltrpc.voltstro.dev)
[![Discord](https://img.shields.io/badge/Discord-Voltstro-7289da.svg?logo=discord)](https://discord.voltstro.dev)

VoltRpc - Library designed for high performance RPC communication.

## Features

- Its fast (See the [benchmarks](#benchmarks))
- Supports the majority of built-in C# types including: 
    - `Bool`, `Byte`, `Char`, `Decimal`, `Double`, `Float`, `Int`, `Long`, `SByte`, `Short`, `String`, `UInt`, `ULong`, `UShort`
    - Array equivalents
- Support [custom types](https://voltrpc.voltstro.dev/articles/types#custom-types) by implementing a ITypeReadWriter
- Proxy generated by using a [.NET Source Generator](https://voltrpc.voltstro.dev/articles/proxy-generation)


## Getting Started

### Installation

VoltRpc can be installed from [NuGet](https://nuget.org/VoltRpc).

You can use the command below to install the package.

```powershell
Install-Package VoltRpc
```

### Example

For a more in-depth example, see the [Overview](https://voltrpc.voltstro.dev/articles/overview) or [Setup](https://voltrpc.voltstro.dev/articles/setup).

There is also a [demo project](/src/Demo) included.

**Shared**:

```csharp
using VoltRpc.Proxy;

namespace VoltRpcExample.Shared
{
    [GenerateProxy(GeneratedName = "TestProxy")]
    public interface ITest
    {
        public void Basic();

        public string Hello();
    }
}
```

**Host**:

```csharp
using System;
using System.Net;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;
using VoltRpcExample.Shared;

namespace VoltRpcExample
{
    public class Program
    {
        public static void Main()
        {
            Host host = new TCPHost(new IPEndPoint(IPAddress.Loopback, 7767));
            host.AddService<ITest>(new TestImpl());
            host.StartListening();

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            host.Dispose();
        }

        public class TestImpl : ITest
        {
            public void Basic()
            {
                Console.WriteLine("Hello!");
            }

            public string Hello()
            {
                return "Hello World!";
            }
        }
    }
}
```

**Client**:

```csharp
using System;
using System.Net;
using VoltRpc.Communication.TCP;
using VoltRpc.Proxy.Generated;
using VoltRpcExample.Shared;

namespace VoltRpcExample.Client
{
    public class Program
    {
        public static void Main()
        {
            VoltRpc.Communication.Client client = new TCPClient(new IPEndPoint(IPAddress.Loopback, 7767));
            client.AddService<ITest>();
            client.Connect();

            ITest testProxy = new TestProxy(client);
            testProxy.Basic();
            Console.WriteLine($"Got from server: {testProxy.Hello()}");

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }
    }
}
```

## Benchmarks

``` ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19043.1110 (21H1/May2021Update)
Intel Core i5-10600KF CPU 4.10GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.302
  [Host]     : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT
  Job-GCFSJA : .NET 5.0.8 (5.0.821.31504), X64 RyuJIT

Jit=Default  Platform=AnyCpu  
```

![Pipes Benchmark](media/PipesBenchmark.png)

|               Method | array         |      message |     Mean |     Error |    StdDev |
|--------------------- |-------------- |------------- |---------:|----------:|----------:|
|            BasicVoid |     ?         |            ? | 6.821 μs | 0.0889 μs | 0.0832 μs |
|          BasicReturn |     ?         |            ? | 6.883 μs | 0.1264 μs | 0.1120 μs |
|   BasicParameterVoid |     ?         | Hello World! | 7.088 μs | 0.0485 μs | 0.0430 μs |
| BasicParameterReturn |     ?         | Hello World! | 7.506 μs | 0.1465 μs | 0.2407 μs |
|          ArrayReturn |     ?         |            ? | 7.024 μs | 0.0449 μs | 0.0420 μs |
|   ArrayParameterVoid |Byte[50]       |            ? | 6.952 μs | 0.0576 μs | 0.0481 μs |
|   ArrayParameterVoid |Byte[8,294,400]|            ? | 6.926 μs | 0.0649 μs | 0.0607 μs |
| ArrayParameterReturn |Byte[50]       |            ? | 7.237 μs | 0.0605 μs | 0.0536 μs |
| ArrayParameterReturn |Byte[8,294,400]|            ? | 7.176 μs | 0.1252 μs | 0.1110 μs |

For more info on these benchmarks see [Benchmarks](https://voltrpc.voltstro.dev/articles/benchmarks).

## Authors

**Voltstro** - *Initial work* - [Voltstro](https://github.com/Voltstro)

## License

This project is licensed under the MIT license – see the [LICENSE.md](/LICENSE.md) file for details.

## Credits

- [Mirror](https://github.com/vis2k/Mirror) 
  - [`NetworkReader.cs`](https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkReader.cs) used as a base for [`BufferedReader.cs`](/src/VoltRpc/IO/BufferedReader.cs)
  - [`NetworkWriter.cs`](https://github.com/vis2k/Mirror/blob/ca4c2fd9302b1ece4240b09cc562e25bcb84407f/Assets/Mirror/Runtime/NetworkWriter.cs) used as a base for [`BufferedWriter.cs`](/src/VoltRpc/IO/BufferedWriter.cs)
- Parts of [`BufferedStream.cs`](https://github.com/dotnet/runtime/blob/release/5.0/src/libraries/System.Private.CoreLib/src/System/IO/BufferedStream.cs) from the .NET Runtime was also used in the reader. 
