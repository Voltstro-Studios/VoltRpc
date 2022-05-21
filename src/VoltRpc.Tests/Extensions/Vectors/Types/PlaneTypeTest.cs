using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class PlaneTypeTest
{
    [Test]
    public void PlaneTest()
    {
        Utils.TestTypeReaderWriter(new PlaneTypeReadWriter(), new Plane(72f, 28f, 129f, 69f));
    }
}