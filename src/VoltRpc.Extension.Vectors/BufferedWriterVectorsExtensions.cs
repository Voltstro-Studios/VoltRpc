using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Extension.Vectors;

/// <summary>
///     Provides System.Numerics.Vectors support to <see cref="BufferedWriter"/>
/// </summary>
public static class BufferedWriterVectorsExtensions
{
    /// <summary>
    ///     Writes a <see cref="Matrix3x2"/>
    /// </summary>
    public static void WriteMatrix3X2(this BufferedWriter writer, Matrix3x2 value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Matrix4x4"/>
    /// </summary>
    public static void WriteMatrix4X4(this BufferedWriter writer, Matrix4x4 value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Plane"/>
    /// </summary>
    public static void WritePlane(this BufferedWriter writer, Plane value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Quaternion"/>
    /// </summary>
    public static void WriteQuaternion(this BufferedWriter writer, Quaternion value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Vector2"/>
    /// </summary>
    public static void WriteVector2(this BufferedWriter writer, Vector2 value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Vector3"/>
    /// </summary>
    public static void WriteVector3(this BufferedWriter writer, Vector3 value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="Vector4"/>
    /// </summary>
    public static void WriteVector4(this BufferedWriter writer, Vector4 value)
    {
        writer.WriteBlittable(value);
    }
}