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

    public static readonly DiagnosticDescriptor InvalidMethodAccessibility = new DiagnosticDescriptor(
        "VRPC02",
        new LocalizableResourceString(nameof(SR.MethodInvalidAccessibility), SR.ResourceManager, typeof(SR)),
        new LocalizableResourceString(nameof(SR.MethodInvalidAccessibilityDescription), SR.ResourceManager, typeof(SR)),
        "Proxy",
        DiagnosticSeverity.Error,
        true);
    
    public static readonly DiagnosticDescriptor InterfaceInvalidAccessibility = new DiagnosticDescriptor(
        "VRPC03",
        new LocalizableResourceString(nameof(SR.InterfaceInvalidAccessibility), SR.ResourceManager, typeof(SR)),
        new LocalizableResourceString(nameof(SR.InterfaceInvalidaccessibilityDescription), SR.ResourceManager, typeof(SR)),
        "Proxy",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor ProxyNameBlank = new DiagnosticDescriptor(
        "VRPC04",
        new LocalizableResourceString(nameof(SR.ProxyNameBlank), SR.ResourceManager, typeof(SR)),
        new LocalizableResourceString(nameof(SR.ProxyNameBlankDescription), SR.ResourceManager, typeof(SR)),
        "Proxy",
        DiagnosticSeverity.Error,
        true);
    
    public static readonly DiagnosticDescriptor ProxyNamespaceBlank = new DiagnosticDescriptor(
        "VRPC05",
        new LocalizableResourceString(nameof(SR.ProxyNamespaceBlank), SR.ResourceManager, typeof(SR)),
        new LocalizableResourceString(nameof(SR.ProxyNamespaceBlankDescription), SR.ResourceManager, typeof(SR)),
        "Proxy",
        DiagnosticSeverity.Error,
        true);
}