using System;

namespace VoltRpc.Types;

/// <summary>
///     Helper methods for <see cref="Type" />s
/// </summary>
internal static class TypesHelper
{
    /// <summary>
    ///     Gets a <see cref="Type" />'s full name
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to get the full name from</param>
    /// <returns>Returns the fullname of the <see cref="Type" /></returns>
    /// <exception cref="ArgumentNullException">Throw if provided type is null</exception>
    /// <exception cref="NullReferenceException">Thrown if type is an array, but it's item type is null!</exception>
    public static string GetTypeName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        
        string typeFullName = type.FullName;
        return typeFullName;
    }
    
    /// <summary>
    ///     Gets a <see cref="Type"/> base, then it's name
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetTypeBaseName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        type = type.GetTypeBase();
        return type.GetTypeName();
    }

    /// <summary>
    ///     Gets the absolute base <see cref="Type" /> of a <see cref="Type" />.
    ///     <para>Checks if refs and arrays</para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static Type GetTypeBase(this Type type)
    {
        Type baseType = type;
        if (type.IsArray || type.IsByRef)
        {
            baseType = type.GetElementType();
            if (baseType == null)
                throw new NullReferenceException("Failed to get type's element type!");

            return baseType.GetTypeBase();
        }

        return baseType;
    }
}