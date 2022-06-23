using System;
using NUnit.Framework;
using VoltRpc.Types;

namespace VoltRpc.Tests.Types;

public struct CustomType
{
}

public class TypesHelperTests
{
    [Test]
    public void GetTypeNameTest()
    {
        string name = typeof(CustomType).GetTypeBaseName();
        StringAssert.AreEqualIgnoringCase("VoltRpc.Tests.Types.CustomType", name);
    }

    [Test]
    public void GetTypeArrayNameTest()
    {
        CustomType[] array = Array.Empty<CustomType>();
        string name = array.GetType().GetTypeBaseName();
        StringAssert.AreEqualIgnoringCase("VoltRpc.Tests.Types.CustomType", name);
    }

    [Test]
    public void GetTypeStringArrayTest()
    {
        string name = typeof(string[]).GetTypeBaseName();
        StringAssert.AreEqualIgnoringCase("System.String", name);
    }
}