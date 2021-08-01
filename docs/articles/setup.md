# Setup

This is a bit more in-depth guide to using VoltRpc.

## Projects

When using VoltRpc, you will usually have three projects:

- Host - The project responsible for hosting and providing the interface.
- Client - User of the provided interface from the host.
- Shared - Provides the `interface` used by both the client and host.

(You could also just have these all in one project similar to how [VoltRpc.Benchmarks](https://github.com/Voltstro-Studios/VoltRpc/tree/master/src/VoltRpc.Benchmarks) does it.)

## Getting Started

First, create these projects list above, use whatever name you want, however I'd recommend something like `<ProjectName>.Client`.

> [!NOTE]
> For this setup we will be using TCP. [Other communication layers](communication-layers.md) are available.

## Installation

You will need to install the [NuGet package](https://nuget.org/VoltRpc) to add VoltRpc to your project.

You can use the command below to install the package.

```powershell
Install-Package VoltRpc
```

## Shared

In the shared project, create an `interface` called `ITest`. You can define whatever methods you want to use, with any return or argument <xref:System.Type> you want, as long as it's a [supported type](types.md#types).

If you want to automatically generate a proxy for the `interface`, follow the [Proxy Generator Guide](proxy-generation.md) on how to use it.

In the end ou should have something that looks like this for the `interface`:

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

## Client

In the client's `Program.cs` file, we will need to use a <xref:VoltRpc.Communication.Client> class to talk to a host.

We have also used the generated `TestProxy` to provide a nice way of calling methods on the server.

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

            //While a lot of other libraries don't require to define an interface this way, we do for caching reasons.
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

## Server

On the server we use a <xref:VoltRpc.Communication.Host> to provide an interface for clients to interact with.

We also need to provide an implementation of our interface that we are using.

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