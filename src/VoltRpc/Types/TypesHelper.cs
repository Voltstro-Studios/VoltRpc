using System;

namespace VoltRpc.Types;

/// <summary>
///     Helper methods for <see cref="Type" />s
/// </summary>
public static class TypesHelper
{
    /// <summary>
    ///     Gets a <see cref="Type" />'s full name
    ///     <para>If the <see cref="Type" /> is an array, it will get the array item's full name</para>
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to get the full name from</param>
    /// <returns>Returns the fullname of the <see cref="Type" /></returns>
    /// <exception cref="ArgumentNullException">Throw if provided type is null</exception>
    /// <exception cref="NullReferenceException">Thrown if type is an array, but it's item type is null!</exception>
    public static string GetTypeName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        type = type.GetTypeBase();
        string typeFullName = type.FullName;
        return typeFullName;
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