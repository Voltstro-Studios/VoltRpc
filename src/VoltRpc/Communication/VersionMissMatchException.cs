using System;

namespace VoltRpc.Communication;

/// <summary>
/// 
/// </summary>
public sealed class VersionMissMatchException : Exception
{
    internal VersionMissMatchException(string message)
        : base(message)
    {
    }
}