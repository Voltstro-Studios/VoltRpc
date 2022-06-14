namespace VoltRpc.Communication;

/// <summary>
///     A response from a message
/// </summary>
public enum MessageResponse : byte
{
    /// <summary>
    ///     The method wasn't found
    /// </summary>
    MethodNotFound,
    
    /// <summary>
    ///     The method was executed successfully
    /// </summary>
    MethodExecutedSuccessful,

    /// <summary>
    ///     The method failed while being invoked
    /// </summary>
    MethodExecuteFailException,
    
    /// <summary>
    ///     Sync was all good mate!
    /// </summary>
    SyncRighto,
    
    /// <summary>
    ///     Response when version info is a miss-match
    /// </summary>
    SyncVersionMissMatch,
    
    /// <summary>
    ///     The existence of a protocol are miss-matched
    /// </summary>
    SyncProtocolExistenceMissMatch,
    
    /// <summary>
    ///     The <see cref="System.Type"/> of the protocol value doesn't match
    /// </summary>
    SyncProtocolTypeMissMatch,
    
    /// <summary>
    ///     The value of the protocol don't match
    /// </summary>
    SyncProtocolValueMissMatch,
    
    /// <summary>
    ///     Some miss-match with services
    /// </summary>
    SyncServiceMissMatch,
    
    /// <summary>
    ///     The method failed to execute due to a missing type reader/writer
    /// </summary>
    TypeReadWriterFailMissing,

    /// <summary>
    ///     The type reader/writer failed for some reason
    /// </summary>
    TypeReadWriterFail
}