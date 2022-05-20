using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Extension.Vectors;

/// <summary>
///    Provides System.Numerics.Vectors support to <see cref="BufferedReader"/>
/// </summary>
public static class BufferedReaderVectorsExtensions
{
    /// <summary>
    ///     Reads a <see cref="Matrix3x2"/>
    /// </summary>
    public static Matrix3x2 ReadMatrix3X2(this BufferedReader reader)
    {
        return reader.ReadBlittable<Matrix3x2>();
    }

    /// <summary>
    ///     Reads a <see cref="Matrix4x4"/>
    /// </summary>
    public static Matrix4x4 ReadMatrix4X4(this BufferedReader reader)
    {
        return reader.ReadBlittable<Matrix4x4>();
    }

    /// <summary>
    ///     Reads a <see cref="Plane"/>
    /// </summary>
    public static Plane ReadPlane(this BufferedReader reader)
    {
        return reader.ReadBlittable<Plane>();
    }

    /// <summary>
    ///     Reads a <see cref="Quaternion"/>
    /// </summary>
    public static Quaternion ReadQuaternion(this BufferedReader reader)
    {
        return reader.ReadBlittable<Quaternion>();
    }

    /// <summary>
    ///     Reads a <see cref="Vector2"/>
    /// </summary>
    public static Vector2 ReadVector2(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector2>();
    }

    /// <summary>
    ///     Reads a <see cref="Vector3"/>
    /// </summary>
    public static Vector3 ReadVector3(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector3>();
    }

    /// <summary>
    ///     Reads a <see cref="Vector4"/>
    /// </summary>
    public static Vector4 ReadVector4(this BufferedReader reader)
    {
        return reader.ReadBlittable<Vector4>();
    }
}