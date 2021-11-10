﻿using System.Runtime.InteropServices;

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