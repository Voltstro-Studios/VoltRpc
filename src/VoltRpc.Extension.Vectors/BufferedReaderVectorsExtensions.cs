using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Extension.Vectors;

/// <summary>
///    Provides System.Numerics.Vectors support to <see cref="BufferedReader"/>
/// </summary>
public static class BufferedReaderVectorsExtensions
{
    public static Matrix3x2 ReadMatrix3X2(this BufferedReader reader)
    {
        return reader.ReadBlittable<Matrix3x2>();
    }

    public static Matrix4x4 ReadMatrix4X4(this BufferedReader reader)
    {
        return reader.ReadBlittable<Matrix4x4>();
    }

    public static Plane ReadPlane(this BufferedReader reader)
    {
        return reader.ReadBlittable<Plane>();
    }

    public static Quaternion ReadQuaternion(this BufferedReader reader)
    {
        return reader.ReadBlittable<Quaternion>();
    }

    public static Vector2 ReadVector2(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector2>();
    }

    public static Vector3 ReadVector3(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector3>();
    }

    public static Vector4 ReadVector4(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector4>();
    }
}