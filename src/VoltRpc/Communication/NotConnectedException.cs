using System;

namespace VoltRpc.Communication;

/// <summary>
///     Thrown when something is not connected
/// </summary>
public class NotConnectedException : Exception
{
    /// <summary>
    ///     Create new <see cref="NotConnectedException" />
    /// </summary>
    /// <param name="message"></param>
    public NotConnectedException(string message)
        : base(message)
    {
    }
}