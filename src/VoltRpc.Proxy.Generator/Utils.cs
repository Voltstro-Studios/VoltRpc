using System;
using Microsoft.CodeAnalysis;

namespace VoltRpc.Proxy.Generator;

public static class Utils
{
    public static string AsString(this Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.NotApplicable:
                return string.Empty;
            case Accessibility.Private:
                return "private";
            case Accessibility.ProtectedAndInternal:
                return "protected internal";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.Internal:
                return "internal";
            case Accessibility.Public:
                return "public";
            default:
                throw new ArgumentOutOfRangeException(nameof(accessibility), accessibility, null);
        }
    }
}