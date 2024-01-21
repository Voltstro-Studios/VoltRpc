using System;
using System.IO;
using System.Numerics;

namespace VoltRpc.IO;

/// <summary>
///     Read methods for <see cref="BufferedReader"/>
/// </summary>
public static class BufferedReaderExtensions
{
    /// <summary>
    ///     Reads a <see cref="sbyte" />
    /// </summary>
    /// <returns></returns>
    public static sbyte ReadSByte(this BufferedReader reader)
    {
        return reader.ReadBlittable<sbyte>();
    }
    
    /// <summary>
    ///     Reads a <see cref="bool" />
    /// </summary>
    /// <returns></returns>
    public static bool ReadBool(this BufferedReader reader)
    {
        return reader.ReadBlittable<byte>() != 0;
    }
    
    /// <summary>
    ///     Reads a <see cref="ushort" />
    /// </summary>
    /// <returns></returns>
    public static ushort ReadUShort(this BufferedReader reader)
    {
        return reader.ReadBlittable<ushort>();
    }
    
    /// <summary>
    ///     Reads a <see cref="short" />
    /// </summary>
    /// <returns></returns>
    public static short ReadShort(this BufferedReader reader)
    {
        return reader.ReadBlittable<short>();
    }

    /// <summary>
    ///     Reads a <see cref="char" />
    /// </summary>
    /// <returns></returns>
    public static char ReadChar(this BufferedReader reader)
    {
        return (char)reader.ReadBlittable<ushort>();
    }

    /// <summary>
    ///     Reads a <see cref="uint" />
    /// </summary>
    /// <returns></returns>
    public static uint ReadUInt(this BufferedReader reader)
    {
        return reader.ReadBlittable<uint>();
    }

    /// <summary>
    ///     Reads a <see cref="int" />
    /// </summary>
    /// <returns></returns>
    public static int ReadInt(this BufferedReader reader)
    {
        return reader.ReadBlittable<int>();
    }

    /// <summary>
    ///     Reads a <see cref="ulong" />
    /// </summary>
    /// <returns></returns>
    public static ulong ReadULong(this BufferedReader reader)
    {
        return reader.ReadBlittable<ulong>();
    }

    /// <summary>
    ///     Reads a <see cref="long" />
    /// </summary>
    /// <returns></returns>
    public static long ReadLong(this BufferedReader reader)
    {
        return reader.ReadBlittable<long>();
    }
    
    /// <summary>
    ///     Reads a <see cref="float" />
    /// </summary>
    /// <returns></returns>
    public static float ReadFloat(this BufferedReader reader)
    {
        return reader.ReadBlittable<float>();
    }

    /// <summary>
    ///     Reads a <see cref="double" />
    /// </summary>
    /// <returns></returns>
    public static double ReadDouble(this BufferedReader reader)
    {
        return reader.ReadBlittable<double>();
    }

    /// <summary>
    ///     Reads a <see cref="decimal" />
    /// </summary>
    /// <returns></returns>
    public static decimal ReadDecimal(this BufferedReader reader)
    {
        return reader.ReadBlittable<decimal>();
    }

    #region Span/Memory Readers

    /// <summary>
    ///     Reads from <see cref="BufferedReader"/> and returns a <see cref="Span{T}"/> slice of the buffer
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> ReadBytesSpanSlice(this BufferedReader reader, int size)
    {
        reader.CheckDispose();
        
        if (reader.Position == reader.readLength)
            reader.ReadStream(size);

        //Check if within buffer limits
        if (reader.Position + size > reader.readLength)
        {
            //Attempt to read again
            reader.ReadStream(size);
            if (reader.Position + size > reader.readLength)
                throw new EndOfStreamException("Cannot read beyond stream!");
        }
        
        Span<byte> buffer = reader.buffer;

        Span<byte> data = buffer.Slice(reader.Position, size);
        reader.Position += size;
        return data;
    }

    /// <summary>
    ///     Reads from <see cref="BufferedReader"/> and returns a <see cref="Span{T}"/> copy of it
    ///     <para>This method does allocate</para>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static ReadOnlySpan<byte> ReadBytesSpanCopy(this BufferedReader reader, int size)
    {
        ReadOnlySpan<byte> slice = reader.ReadBytesSpanSlice(size);

        Span<byte> copy = new byte[size];
        slice.CopyTo(copy);
        
        return copy;
    }

    /// <summary>
    ///     Reads a <see cref="string"/>, but is using <see cref="ReadBytesSpanSlice"/> instead
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="EndOfStreamException"></exception>
    public static string? ReadStringSpan(this BufferedReader reader)
    {
        ushort size = reader.ReadUShort();
        if (size == 0)
            return null;

        int realSize = size - 1;

        //Make sure it's within limits to avoid allocation attacks etc.
        if (realSize >= BufferedWriter.MaxStringLength)
            throw new EndOfStreamException(
                $"Read string was too long! Max size is {BufferedWriter.MaxStringLength}.");

        ReadOnlySpan<byte> data = reader.ReadBytesSpanSlice(realSize);
        return reader.encoding.GetString(data);
    }

    #endregion

    #region Vector Readers

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

    #endregion
}