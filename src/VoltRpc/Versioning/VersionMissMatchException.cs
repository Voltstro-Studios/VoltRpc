using System;

namespace VoltRpc.Versioning;

/// <summary>
///     <see cref="Exception"/> related to when there is a version miss-match between
///     the <see cref="VoltRpc.Communication.Host"/> and <see cref="VoltRpc.Communication.Client"/>
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