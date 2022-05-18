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
}