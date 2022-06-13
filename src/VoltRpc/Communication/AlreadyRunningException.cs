using System;

namespace VoltRpc.Communication;

/// <summary>
///     <see cref="Exception"/> related to when something is already running
/// </summary>
public sealed class AlreadyRunningException : Exception
{
}