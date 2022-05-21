using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.Types;

namespace VoltRpc.Tests.Extensions.Vectors.Types;

public class Matrix4X4TypeTest
{
    [Test]
    public void Matrix4X4Test()
    {
        Utils.TestTypeReaderWriter(new Matrix4X4TypeReadWriter(), Matrix4x4.CreateRotationZ(56f));
    }
}