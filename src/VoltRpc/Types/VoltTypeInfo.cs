using System;

namespace VoltRpc.Types;

/// <summary>
///     Contains information the VoltRpc needs on a <see cref="Type"/>
/// </summary>
internal class VoltTypeInfo
{
    /// <summary>
    ///     Creates a new <see cref="VoltTypeInfo"/>
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="NullReferenceException"></exception>
    internal VoltTypeInfo(Type type)
    {
        BaseType = type.GetTypeBase();
        TypeName = BaseType.GetTypeName();
        
        if (type.IsByRef)
        {
            Type baseType = type.GetElementType();
            if (baseType == null)
                throw new NullReferenceException("Failed to get type's element type!");
            IsArray = baseType.IsArray;
        }
        else
        {
            IsArray = type.IsArray;
        }
    }

    /// <summary>
    ///     The base <see cref="Type"/>
    /// </summary>
    public Type BaseType { get; }

    /// <summary>
    ///     The fullname of the <see cref="Type"/>
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    ///     Is this <see cref="Type"/> an array?
    /// </summary>
    public bool IsArray { get; }
}