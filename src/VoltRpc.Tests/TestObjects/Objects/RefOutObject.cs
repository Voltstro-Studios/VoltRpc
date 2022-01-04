using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class RefOutObject : IRefOutInterface
{
    public void RefOutBasic(ref int value, out int outValue)
    {
        Assert.AreEqual(128, value);
        value = 75;
        outValue = 25;
    }
}