using System;

namespace VoltRpc.Communication;

/// <summary>
/// 
/// </summary>
public sealed class VersionMissMatchException : Exception
{
    internal VersionMissMatchException(Version version)
        : base($"Version miss-match! Host excepting version {version.ToString()}")
    {
        ExceptedVersion = version;
    }
    
    /// <summary>
    ///     What the excepted version was
    /// </summary>
    public Version ExceptedVersion { get; }
}