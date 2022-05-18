using System.IO;

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
}