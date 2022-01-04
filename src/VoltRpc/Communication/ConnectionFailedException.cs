using System;

namespace VoltRpc.Communication;

/// <summary>
///     An <see cref="Exception" /> related to a connection failing
/// </summary>
public class ConnectionFailedException : Exception
{
    /// <summary>
    ///     Creates a new <see cref="ConnectionFailedException" /> instance
    /// </summary>
    /// <param name="message"></param>
    public ConnectionFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Creates a new <see cref="ConnectionFailedException" /> instance
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public ConnectionFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}