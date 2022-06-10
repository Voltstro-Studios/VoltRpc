using System;

namespace VoltRpc;

/// <summary>
///     VoltRpc versioning information
/// </summary>
public static class Versioning
{
    static Versioning()
    {
        if(VersionSet)
            return;

        Version version = System.Version.Parse(ThisAssembly.Info.InformationalVersion);
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
        public byte Major { get; internal set; }
    
        /// <summary>
        ///     Minor version number
        /// </summary>
        public byte Minor { get; internal set; }
    
        /// <summary>
        ///     Patch version number
        /// </summary>
        public byte Patch { get; internal set; }
    }
}