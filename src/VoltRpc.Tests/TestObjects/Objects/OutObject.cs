using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class OutObject : IOutInterface
{
    public void OutBasic(out int value)
    {
        value = 128;
    }
}