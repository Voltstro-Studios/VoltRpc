namespace VoltRpc.Communication
{
    /// <summary>
    ///     What type of message is this?
    /// </summary>
    public enum MessageType
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
}