# Proxy Generation

For generation of proxies, a [.NET Source generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) is used. If you don't know what what a .NET Source generator is, they are feature in the Roslyn compiler to generate source code based of the code in your project.

> [!NOTE]
> You may need to restart your IDE when you first add the generator for it to work.<br>
> Sometimes the IDE will also just shit it self (particularly Rider) and refuse to acknowledge any generated code, causing the intellisense to mark anything generated as an error. Compiling will be fine tho.
> Hopefully as this feature gets more flushed out these issues won't occur anymore.

## Install

You will need to install the [NuGet package](https://www.nuget.org/packages/VoltRpc.Proxy.Generator/) to add the generator to your project.

To install it, you can add it to your project's `csproj` like so:

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc.Proxy.Generator" Version="2.3.0" />
</ItemGroup>
```

## Usage

The generator will generate proxies for any `interface` marked with an `GenerateProxy` attribute. By default the generator will generate classes with the name `<InterfaceName>_GeneratedProxy` and with the namespace `VoltRpc.Proxy.Generated`. To override both of these, use the `GeneratedName` to set what the name should be, and `GeneratedNamespace` to set what the namespace should be.

For example:
```csharp
using VoltRpc.Proxy;

namespace VoltRpcExample.Shared
{
    [GenerateProxy(GeneratedName = "TestProxy", GeneratedNamespace = "VoltRpcExample.Shared")]
    public interface ITest
    {
        public void Basic();

        public string Hello();
    }
}
```

This would generate the proxy with the name set to `TestProxy`, and with the namespace set to `VoltRpcExample.Shared`.