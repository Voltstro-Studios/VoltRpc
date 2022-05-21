using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class Matrix3X3TypeTest
{
    [Test]
    public void Matrix3X2Test()
    {
        Utils.TestTypeReaderWriter(new Matrix3X2TypeReadWriter(), Matrix3x2.CreateRotation(128f));
    }
}