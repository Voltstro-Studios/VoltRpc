using VoltRpc.Proxy;

namespace VoltRpc.Benchmarks.Interface;

[GenerateProxy(GeneratedName = "BenchmarkProxy")]
public interface IBenchmarkInterface
{
    public void BasicVoid();

    public void BasicParameterVoid(string message);

    public string BasicReturn();

    public string BasicParameterReturn(string message);

    public void ArrayParameterVoid(byte[] array);

    public byte[] ArrayReturn();

    public byte[] ArrayParameterReturn(byte[] array);
}