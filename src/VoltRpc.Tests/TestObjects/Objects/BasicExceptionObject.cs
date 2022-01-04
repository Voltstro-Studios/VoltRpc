using System;
using VoltRpc.Tests.TestObjects.Interfaces;

namespace VoltRpc.Tests.TestObjects.Objects;

public class BasicExceptionObject : IBasicInterface
{
    public void Basic()
    {
        throw new Exception("Hello World!");
    }
}