using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class QuaternionTypeTest
{
    [Test]
    public void QuaternionTest()
    {
        Utils.TestTypeReaderWriter(new QuaternionTypeReadWriter(), Quaternion.CreateFromAxisAngle(Vector3.UnitX, 27f));
    }
}