using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class RefOutReturnObject : IRefOutReturnInterface
{
    public int RefOutReturn(ref int value, out int outValue)
    {
        Assert.AreEqual(128, value);
        value = 75;
        outValue = 25;
        return 16;
    }
}