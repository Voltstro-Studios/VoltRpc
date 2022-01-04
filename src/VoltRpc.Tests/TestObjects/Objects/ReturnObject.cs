using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class ReturnObject : IReturnInterface
{
    public int ReturnBasic()
    {
        return 128;
    }
}