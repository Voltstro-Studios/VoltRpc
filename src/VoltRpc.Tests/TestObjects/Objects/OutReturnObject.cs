using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class OutReturnObject : IOutReturnInterface
{
    public int OutReturn(out int value)
    {
        value = 128;
        return 75;
    }
}