<div class="hero container">
    <div class="hero-content">
        <h1>VoltRpc</h1>
        <div class="lead">
            <p>An RPC library which is designed to be both simple to use and fast.</p>
        </div>
        <div class="actions">
            <a href="articles/" class="btn btn-primary btn-lg">Get Started</a>
        </div>
    </div>
</div>


### Install VoltRpc

Add to your project's `csproj`:

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc" Version="3.2.0" />
    <PackageReference Include="VoltRpc.Proxy.Generator" Version="2.2.0" />
</ItemGroup>
```

### And Start Using!

```csharp
[GenerateProxy(GeneratedName = "TestProxy")]
public interface ITestInterface
{
    public void DoSomethingCool();
    public int GetTheCoolValue();
}

public class TestInterface : ITestInterface
{
    public void DoSomethingCool()
    {
        Console.WriteLine("Something Cool!");
    }

    public int GetTheCoolValue()
    {
        return 69;
    }
}

public class Program
{
    IPEndPoint ip = new(IPAddress.Loopback, 7767);

    TestInterface test = new();

    //Host
    Host host = new TCPHost(ip);
    host.AddService<ITestInterface>(test);
    host.StartListening().ConfigureAwait(false);

    //Client
    Client client = new TCPClient(ip);
    client.AddService<ITestInterface>();
    client.Connect();

    //Now we can call to method like it was normal C#
    TestProxy proxy = new(client);
    proxy.DoSomethingCool();
}
```
