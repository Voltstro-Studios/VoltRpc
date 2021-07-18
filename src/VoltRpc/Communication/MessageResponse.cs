namespace VoltRpc.Communication
{
    /// <summary>
    ///     A response from a message
    /// </summary>
    public enum MessageResponse
    {
        /// <summary>
        ///     The method wasn't found
        /// </summary>
        NoMethodFound,
        
        /// <summary>
        ///     The method was executed successfully
        /// </summary>
        ExecutedSuccessful,
        
        /// <summary>
        ///     The method failed to execute due to a missing type reader/writer
        /// </summary>
        ExecuteFailNoTypeReader,
        
        /// <summary>
        ///     The type reader/writer failed for some reason
        /// </summary>
        ExecuteTypeReadWriteFail,
        
        /// <summary>
        ///     The method failed while being invoked
        /// </summary>
        ExecuteInvokeFailException
    }
}