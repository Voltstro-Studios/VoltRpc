namespace VoltRpc.Communication;

/// <summary>
///     What type of message is this?
/// </summary>
internal enum MessageType : byte
{
    /// <summary>
    ///     Invoke a message
    /// </summary>
    InvokeMethod,

    /// <summary>
    ///     Shutdown the connection
    /// </summary>
    Shutdown
}