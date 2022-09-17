using VoltRpc.Proxy;

namespace VoltRpc.Demo.Shared;

[GenerateProxy(GeneratedName = "InternalTest", ForcePublic = false)]
internal interface IInternalTest
{
    public void Hi();
}
