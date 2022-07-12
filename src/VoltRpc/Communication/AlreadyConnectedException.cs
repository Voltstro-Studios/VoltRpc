using System;

namespace VoltRpc.Communication;

/// <summary>
///     An <see cref="Exception"/> related to when an action is attempted to be done that cannot be done while the
///     <see cref="Client"/> is already connected
/// </summary>
public sealed class AlreadyConnectedException : Exception
{
}