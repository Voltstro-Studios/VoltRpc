# Setup

This is a bit more in-depth guide to using VoltRpc.

## Projects

When using VoltRpc, you will usually have three projects:

- Host - The project responsible for hosting and providing the interface.
- Client - User of the provided interface from the host.
- Shared - Provides the `interface` used by both the client and host.

(You could also just have these all in one project similar to how [VoltRpc.Benchmarks](https://github.com/Voltstro-Studios/VoltRpc/tree/master/src/VoltRpc.Benchmarks) does it.)

## Getting Started

First, create the projects listed above, use whatever name you want, however I'd recommend something like `<ProjectName>.Client`.

> [!NOTE]
> For this setup we will be using TCP. [Other communication layers](communication-layers.md) are available.

## Installation

You will need to install the [VoltRpc NuGet package](https://www.nuget.org/packages/VoltRpc/) and VoltRpc's proxy .NET source generator. More info on the generator can be [found on it's page](proxy-generation.md).

You can add the NuGet packages using your IDE's NuGet package manager, or by adding it to your project's `.csproj` file.

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc" Version="3.2.0" />
    <PackageReference Include="VoltRpc.Proxy.Generator" Version="2.3.0" />
</ItemGroup>
```

### Shared

In the shared project, create an `interface` called `ITest`. You can define whatever methods you want to use, with any return or argument <xref:System.Type> you want, as long as it's a [supported type](types.md#types).

We will tell VoltRpc's proxy generator that we want a proxy for this interface (as we want to use it), so add the <xref:VoltRpc.Proxy.GenerateProxyAttribute> to it. The attribute has some settings to be changed about the generated result, for more info see he [Proxy Generator Guide](proxy-generation.md) on how to use it.

In the end we should have something that looks like this for our `interface`:

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

### Client

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

            //This will call the Basic() method on the host
            testProxy.Basic();

            //This will call the Hello() method on the host, and print out the result of it
            Console.WriteLine($"Got from host: {testProxy.Hello()}");

            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();

            client.Dispose();
        }
    }
}
```

### Host

On the host we use a <xref:VoltRpc.Communication.Host> to provide an interface for clients to interact with.

We also need to provide an implementation of our interface that we are using. The code in the host's implementation is what will be run when the client calls it, it will be executed on the host itself. So when the client calls `Basic()`, the host's console will print out `"Hello"`.

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

            //This is the async version of the StartListening() method
            host.StartListeningAsync().ConfigureAwait(false);

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

## Demo

If you want to see a more full-on demo, see the [demo that is included in the VoltRpc GitHub repo](https://github.com/Voltstro-Studios/VoltRpc/tree/master/src/Demo). Please keep in mind that this project is what we use for sandbox testing VoltRpc, and is a little bit more complex than what is shown in this guide.