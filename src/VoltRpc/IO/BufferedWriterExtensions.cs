using System;
using System.Numerics;

namespace VoltRpc.IO;

/// <summary>
///     Provides extensions for <see cref="BufferedWriter"/>
/// </summary>
public static class BufferedWriterExtensions
{
     /// <summary>
    ///     Writes a <see cref="sbyte" />
    /// </summary>
     public static void WriteSByte(this BufferedWriter writer, sbyte value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="bool" />
    /// </summary>
    public static void WriteBool(this BufferedWriter writer, bool value)
    {
        writer.WriteBlittable((byte)(value ? 1 : 0));
    }

    /// <summary>
    ///     Writes a <see cref="ushort" />
    /// </summary>
    public static void WriteUShort(this BufferedWriter writer, ushort value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="short" />
    /// </summary>
    public static void WriteShort(this BufferedWriter writer, short value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="char" />
    /// </summary>
    public static void WriteChar(this BufferedWriter writer, char value)
    {
        writer.WriteBlittable((ushort)value);
    }

    /// <summary>
    ///     Writes a <see cref="uint" />
    /// </summary>
    public static void WriteUInt(this BufferedWriter writer, uint value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="int" />
    /// </summary>
    public static void WriteInt(this BufferedWriter writer, int value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="ulong" />
    /// </summary>
    public static void WriteULong(this BufferedWriter writer, ulong value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="long" />
    /// </summary>
    public static void WriteLong(this BufferedWriter writer, long value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="float" />
    /// </summary>
    public static void WriteFloat(this BufferedWriter writer, float value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="double" />
    /// </summary>
    public static void WriteDouble(this BufferedWriter writer, double value)
    {
        writer.WriteBlittable(value);
    }

    /// <summary>
    ///     Writes a <see cref="decimal" />
    /// </summary>
    public static void WriteDecimal(this BufferedWriter writer, decimal value)
    {
        writer.WriteBlittable(value);
    }

    #region Span/Memory Writers

    /// <summary>
    ///     Writes a <see cref="Span{T}"/> <see cref="byte"/> to a <see cref="BufferedWriter"/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    public static void WriteBytesSpan(this BufferedWriter writer, ReadOnlySpan<byte> value)
    {
        writer.CheckDispose();
        
        writer.EnsureCapacity(writer.Position + value.Length);
        
        value.CopyTo(writer.buffer);
        writer.Position += value.Length;
    }

    /// <summary>
    ///     Writes a <see cref="Memory{T}"/> <see cref="byte"/> to a <see cref="BufferedWriter"/>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    public static void WriteBytesMemory(this BufferedWriter writer, ReadOnlyMemory<byte> value)
    {
        writer.WriteBytesSpan(value.Span);
    }

    #endregion

    #region Vector Writers

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

    #endregion
}