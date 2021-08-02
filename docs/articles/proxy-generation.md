# Proxy Generation

For generation of proxies, a [.NET Source generator](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) is used. If you don't know what what a .NET Source generator is, they are feature in the Roslyn compiler to generate source code and include whatever code is generated in the compilation.

> [!NOTE]
> You may need to restart your IDE when you first add the generator for it to work.<br>
> Sometimes the IDE will also just shit it self (particularly Rider) and refuse to acknowledge any generated code, causing the intellisense to mark anything generated as an error. Compiling will be fine tho.
> Hopefully as this feature gets more flushed out these issues won't occur anymore.

## Install

You will need to install the [NuGet package](https://www.nuget.org/packages/VoltRpc.Proxy.Generator/) to add the generator to your project.

You can use the command below to install the package.

```powershell
Install-Package VoltRpc.Proxy.Generator
``` 

## Usage

The generator will generate proxies for any `interface` marked with an `GenerateProxy` attribute. By default the generator will generate classes with the name `<InterfaceName>_GeneratedProxy`. To override this, use the `GeneratedName` to set what the name will be.

For example:
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

This would generate the proxy with the name `TestProxy`.