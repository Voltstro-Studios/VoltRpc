using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class Vector2TypeTest
{
    [Test]
    public void Vector2Test()
    {
        Utils.TestTypeReaderWriter(new Vector2TypeReadWriter(), new Vector2(27f, 456f));
    }
}