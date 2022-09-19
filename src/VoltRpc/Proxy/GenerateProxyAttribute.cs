#nullable enable
using System;

namespace VoltRpc.Proxy;

/// <summary>
///     Marks an interface to have a proxy generated for
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class GenerateProxyAttribute : Attribute
{
    /// <summary>
    ///     Creates a new <see cref="GenerateProxyAttribute"/> instance
    /// </summary>
    public GenerateProxyAttribute()
    {
    }

    /// <summary>
    ///     What name to use for the generated proxy.
    ///     <para>
    ///         By default, the generated proxy name will be {Interface Name}_GeneratedProxy.
    ///     </para>
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? GeneratedName { get; set; }
    
    /// <summary>
    ///     What namespace to use for the generated proxy.
    ///     <para>
    ///         By default, the generated proxy namespace will be VoltRpc.Proxy.Generated.
    ///     </para>
    /// </summary>
    public string? GeneratedNamespace { get; set; }
    
    /// <summary>
    ///     Forces the generated proxy to be public
    ///     <para>
    ///         By default, the generated proxy visibility will be the same as the interfaces
    ///     </para>
    /// </summary>
    public bool ForcePublic { get; set; }
}