using Microsoft.CodeAnalysis;

namespace VoltRpc.Proxy.Generator;

/// <summary>
///     Helper for some diagnostic reporting stuff
/// </summary>
public static class DiagnosticHelper
{
    public static readonly DiagnosticDescriptor NotAMethodError = new DiagnosticDescriptor(
        "VRPC01",
        new LocalizableResourceString(nameof(SR.NotAMethod), SR.ResourceManager, typeof(SR)),
        new LocalizableResourceString(nameof(SR.NotAMethodDescription), SR.ResourceManager, typeof(SR)),
        "Proxy",
        DiagnosticSeverity.Error,
        true);
}