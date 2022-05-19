using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Extensions.Vectors;

public class BufferedReaderVectorsExtensionTests
{
    [Test]
    public void ReadMatrix3X2Test()
    {
        using DualBuffers buffers = new();

        Matrix3x2 matrix3X2 = new Matrix3x2(14, 18, 22, 17, 18, 124);
        buffers.BufferedWriter.WriteMatrix3X2(matrix3X2);
        buffers.BufferedWriter.Flush();

        Matrix3x2 result = buffers.BufferedReader.ReadMatrix3X2();
        Assert.AreEqual(matrix3X2, result);
    }
    
    [Test]
    public void ReadMatrix4X4Test()
    {
        using DualBuffers buffers = new();

        Matrix4x4 matrix4X4 = new Matrix4x4(14, 18, 22, 17, 18, 124, 8, 2, 454, 33, 1, 4389, 4, 4, 4, 7);
        buffers.BufferedWriter.WriteMatrix4X4(matrix4X4);
        buffers.BufferedWriter.Flush();

        Matrix4x4 result = buffers.BufferedReader.ReadMatrix4X4();
        Assert.AreEqual(matrix4X4, result);
    }

    [Test]
    public void ReadPlaneTest()
    {
        using DualBuffers buffers = new();
        
        Plane plane = new Plane(128, 62, 45, 8);
        buffers.BufferedWriter.WritePlane(plane);
        buffers.BufferedWriter.Flush();

        Plane result = buffers.BufferedReader.ReadPlane();
        Assert.AreEqual(plane, result);
    }

    [Test]
    public void ReadQuaternionTest()
    {
        using DualBuffers buffers = new();
        
        Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitX, 32f);
        buffers.BufferedWriter.WriteQuaternion(quaternion);
        buffers.BufferedWriter.Flush();

        Quaternion result = buffers.BufferedReader.ReadQuaternion();
        Assert.AreEqual(quaternion, result);
    }

    [Test]
    public void ReadeVector2()
    {
        using DualBuffers buffers = new();
        
        Vector2 vector2 = Vector2.One;
        buffers.BufferedWriter.WriteVector2(vector2);
        buffers.BufferedWriter.Flush();

        Vector2 result = buffers.BufferedReader.ReadVector2();
        Assert.AreEqual(vector2, result);
    }
    
    [Test]
    public void ReadVector3()
    {
        using DualBuffers buffers = new();
        
        Vector3 vector3 = Vector3.One;
        buffers.BufferedWriter.WriteVector3(vector3);
        buffers.BufferedWriter.Flush();

        Vector3 result = buffers.BufferedReader.ReadVector3();
        Assert.AreEqual(vector3, result);
    }
    
    [Test]
    public void ReadVector4()
    {
        using DualBuffers buffers = new();
        
        Vector4 vector4 = Vector4.One;
        buffers.BufferedWriter.WriteVector4(vector4);
        buffers.BufferedWriter.Flush();

        Vector4 result = buffers.BufferedReader.ReadVector4();
        Assert.AreEqual(vector4, result);
    }
}