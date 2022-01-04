using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class RefReturnObject : IRefReturnInterface
{
    public int RefReturn(ref int value)
    {
        Assert.AreEqual(75, value);
        value = 128;

        return 25;
    }
}