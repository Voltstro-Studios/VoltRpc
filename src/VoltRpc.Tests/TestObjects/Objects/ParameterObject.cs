using NUnit.Framework;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class ParameterObject : IParameterBasicInterface
{
    public void BasicParam(int param)
    {
        Assert.AreEqual(128, param);
    }
}