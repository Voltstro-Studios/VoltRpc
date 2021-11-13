# Overview

## Installation

VoltRpc can be installed via [NuGet](https://www.nuget.org/packages/VoltRpc/).

You can use the command below to install the package.

```powershell
Install-Package VoltRpc
```

You will also want to install the [VoltRpc.Proxy.Generator](https://www.nuget.org/packages/VoltRpc.Proxy.Generator/) package.

You can install that package with the command below.

```powershell
Install-Package VoltRpc.Proxy.Generator
```

## Proxy Generation

All proxy generation is handled by a .NET Source generator. You will need to install it's [NuGet package](https://www.nuget.org/packages/VoltRpc.Proxy.Generator/).

You can use the command below to install the package.

```powershell
Install-Package VoltRpc.Proxy.Generator
```

## Basic setup

Setting up VoltRpc is easy. VoltRpc has three parts; the host, the client and the proxy for the client.

For a more in-depth setup, see [Setup](setup.md).

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