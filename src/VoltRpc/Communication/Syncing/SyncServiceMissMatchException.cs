using System;

namespace VoltRpc.Communication.Syncing;

/// <summary>
///     An <see cref="Exception"/> related to an error with syncing
///     <para>
///         The <see cref="SyncServiceMissMatchException"/> is thrown when the interfaces that the <see cref="Host"/>
///         and <see cref="Client"/> use have differences
///     </para>
/// </summary>
public sealed class SyncServiceMissMatchException : Exception
{
    internal SyncServiceMissMatchException(string message)
        : base(message)
    {
    }
}