using System;

namespace VoltRpc.Communication.Protocol;

/// <summary>
///     <see cref="Exception"/> related to a protocol error
/// </summary>
public sealed class ProtocolException : Exception
{
    internal ProtocolException(string message)
        : base(message)
    {
    }
}