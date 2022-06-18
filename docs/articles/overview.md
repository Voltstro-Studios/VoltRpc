# Overview

Hello, and welcome to the VoltRpc documentation! These docs should hopefully contain the information you need in-order to use VoltRpc.

## What is VoltRpc?

VoltRpc is a [remote procedure call](https://en.wikipedia.org/wiki/Remote_procedure_call) (RPC) library, using it's own custom implementation, which allows VoltRpc to be [fast](benchmarks.md) and easy to use.

This means you can call a C# method on a remote target (such as a separate process) using C# code.

VoltRpc's lowest .NET target is `netstandard2.0`, so you can use any [.NET implementation](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#select-net-standard-version) that implements it.

## Installation

VoltRpc can be installed from [NuGet](https://www.nuget.org/packages/VoltRpc/). You will also want VoltRpc's companion proxy .NET source generator. More info on the generator can be [found on it's page](proxy-generation.md).

You can add the NuGet packages using your IDE's NuGet package manager, or by adding it to your project's `.csproj` file.

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc" Version="3.0.0" />
    <PackageReference Include="VoltRpc.Proxy.Generator" Version="2.1.0" />
</ItemGroup>
```

### Unity

If you want to use VoltRpc with Unity, then install it using [xoofx](https://github.com/xoofx)'s [UnityNuGet](https://github.com/xoofx/UnityNuGet#unitynuget-) project. The packages are called `org.nuget.voltrpc`.

As Unity's .NET source generator compatibility is a bit... ifffy, the .NET source generator package has NOT been added to it.

## Basic setup

Setting up VoltRpc is easy. VoltRpc has three parts; the host, the client and the proxy for the client.

For this setup we will be using TCP. [Other communication layers](communication-layers.md) are available.

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

For a more in-depth setup, see the [Setup](setup.md) section.