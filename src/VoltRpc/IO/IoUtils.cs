using System;
using System.Runtime.CompilerServices;

namespace VoltRpc.IO;

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