using System;

namespace VoltRpc.Versioning;

/// <summary>
///     Contains version info on the VoltRpc library
/// </summary>
public static class LibVersion
{
    static LibVersion()
    {
        if(VersionSet)
            return;

        Version version = System.Version.Parse(ThisAssembly.AssemblyVersion);
        Version = new VersionInfo
        {
            Major = (byte)version.Major,
            Minor = (byte)version.Minor,
            Patch = (byte)version.Build
        };
        
        VersionSet = true;
    }
    
    private static readonly bool VersionSet;
    
    /// <summary>
    ///     Current <see cref="VersionInfo"/> about VoltRpc
    /// </summary>
    public static VersionInfo Version { get; }

    //NOTE: Version numbers are stored as bytes (meaning that can only go upto 255), however I doubt we will have 255 versions, and if we do, then just change it to ints
    /// <summary>
    ///     Info on the VoltRpc version
    /// </summary>
    public struct VersionInfo
    {
        /// <summary>
        ///     Major version number
        /// </summary>
        public byte Major { get; internal init; }
    
        /// <summary>
        ///     Minor version number
        /// </summary>
        public byte Minor { get; internal init; }
    
        /// <summary>
        ///     Patch version number
        /// </summary>
        public byte Patch { get; internal init; }
    }
}