using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoltRpc.IO;

[StructLayout(LayoutKind.Explicit)]
internal struct UIntFloat
{
    [FieldOffset(0)] public float floatValue;

    [FieldOffset(0)] public uint intValue;
}

[StructLayout(LayoutKind.Explicit)]
internal struct UIntDouble
{
    [FieldOffset(0)] public double doubleValue;

    [FieldOffset(0)] public ulong longValue;
}

[StructLayout(LayoutKind.Explicit)]
internal struct UIntDecimal
{
    [FieldOffset(0)] public ulong longValue1;

    [FieldOffset(8)] public ulong longValue2;

    [FieldOffset(0)] public decimal decimalValue;
}

/// <summary>
///     Provides utils that are shared with IO stuff
/// </summary>
internal static class IoUtils
{
    /// <summary>
    ///     Creates a byte array designed for buffers
    /// </summary>
    /// <param name="lenght"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] CreateBuffer(int lenght)
    {
#if NET6_0_OR_GREATER
        return GC.AllocateArray<byte>(lenght, true);
#else
        return new byte[lenght];
#endif
    }
}