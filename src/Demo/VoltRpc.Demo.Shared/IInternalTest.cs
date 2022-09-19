using VoltRpc.Proxy;

namespace VoltRpc.Demo.Shared;

[GenerateProxy(GeneratedName = "InternalTest", GeneratedNamespace = null, ForcePublic = false)]
internal interface IInternalTest
{
    public void Hi();
}
