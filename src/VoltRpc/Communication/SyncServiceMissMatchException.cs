using System;

namespace VoltRpc.Communication;

/// <summary>
///     An <see cref="Exception"/> related to an error with syncing
/// </summary>
public sealed class SyncServiceMissMatchException : Exception
{
    internal SyncServiceMissMatchException(string message)
        : base(message)
    {
    }
}