using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class RefObject : IRefBasicInterface
{
    public void RefBasic(ref int testValue)
    {
        Assert.AreEqual(75, testValue);
        testValue = 128;
    }
}