using VoltRpc.Proxy;

namespace VoltRpc.Demo.Shared;

[GenerateProxy(GeneratedName = "TestProxy")]
public interface ITest
{
    public void BasicTest();

    public void ParmTest(string message, float num);

    public string ReturnTest();

    public void ArrayTest(string[] array);

    public void RefTest(ref string refTest);

    public byte RefReturnTest(ref uint refTest);

    public void OutTest(out string outTest);

    public void CustomTypeTest(CustomType customType);

    public CustomType CustomTypeReturnTest();
}