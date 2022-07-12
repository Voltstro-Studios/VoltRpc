using System;

namespace VoltRpc.Communication;

/// <summary>
///     An <see cref="Exception"/> related to when a method is attempted to be called,
///     but the <see cref="Client"/> has not connected yet
/// </summary>
public class NotConnectedException : Exception
{
    /// <summary>
    ///     Create new <see cref="NotConnectedException" />
    /// </summary>
    /// <param name="message"></param>
    internal NotConnectedException(string message)
        : base(message)
    {
    }
}