using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors;
using VoltRpc.IO;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Extensions.Vectors;

public class BufferedWriterVectorsExtensionTests
{
    [Test]
    public void WriteMatrix3X2Test()
    {
        using DualBuffers buffers = new();

        Matrix3x2 matrix3X2 = new Matrix3x2(14, 18, 22, 17, 18, 124);
        buffers.BufferedWriter.WriteMatrix3X2(matrix3X2);

        CheckCorrectSize<Matrix3x2>(buffers.BufferedWriter);
    }
    
    [Test]
    public void WriteMatrix4X4Test()
    {
        using DualBuffers buffers = new();

        Matrix4x4 matrix4X4 = new Matrix4x4(14, 18, 22, 17, 18, 124, 8, 2, 454, 33, 1, 4389, 4, 4, 4, 7);
        buffers.BufferedWriter.WriteMatrix4X4(matrix4X4);

        CheckCorrectSize<Matrix4x4>(buffers.BufferedWriter);
    }

    [Test]
    public void WritePlaneTest()
    {
        using DualBuffers buffers = new();
        
        Plane plane = new Plane(128, 62, 45, 8);
        buffers.BufferedWriter.WritePlane(plane);

        CheckCorrectSize<Plane>(buffers.BufferedWriter);
    }

    [Test]
    public void WriteQuaternionTest()
    {
        using DualBuffers buffers = new();
        
        Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 32f);
        buffers.BufferedWriter.WriteQuaternion(quaternion);
        
        CheckCorrectSize<Quaternion>(buffers.BufferedWriter);
    }

    [Test]
    public void WriteVector2()
    {
        using DualBuffers buffers = new();
        
        Vector2 vector2 = Vector2.One;
        buffers.BufferedWriter.WriteVector2(vector2);
        
        CheckCorrectSize<Vector2>(buffers.BufferedWriter);
    }
    
    [Test]
    public void WriteVector3()
    {
        using DualBuffers buffers = new();
        
        Vector3 vector3 = Vector3.One;
        buffers.BufferedWriter.WriteVector3(vector3);
        
        CheckCorrectSize<Vector3>(buffers.BufferedWriter);
    }
    
    [Test]
    public void WriteVector4()
    {
        using DualBuffers buffers = new();
        
        Vector4 vector4 = Vector4.One;
        buffers.BufferedWriter.WriteVector4(vector4);
        
        CheckCorrectSize<Vector4>(buffers.BufferedWriter);
    }

    private static unsafe void CheckCorrectSize<T>(BufferedWriter writer)
        where T : unmanaged
    {
        int size = sizeof(T);
        Assert.AreEqual(size, writer.Position);
    }
}