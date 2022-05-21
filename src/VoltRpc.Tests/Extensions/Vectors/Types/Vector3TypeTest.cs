using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class Vector3TypeTest
{
    [Test]
    public void Vector3Test()
    {
        Utils.TestTypeReaderWriter(new Vector3TypeReadWriter(), new Vector3(27f, 38f, 7000f));
    }
}