using System;

namespace VoltRpc.Communication.Syncing;

/// <summary>
///     An <see cref="Exception"/> related to a protocol sync error
///     <para>
///         This could be due to the set protocol being different between the <see cref="Host"/> and
///         <see cref="Client"/>, either from different <see cref="VoltRpc.Types.TypeReadWriter{T}"/> or different
///         values
///     </para>
/// </summary>
public sealed class ProtocolSyncException : Exception
{
    internal ProtocolSyncException(string message)
        : base(message)
    {
    }
}