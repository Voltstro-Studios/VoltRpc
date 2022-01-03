using System;

namespace VoltRpc.Types;

internal class VoltTypeInfo
{
    public VoltTypeInfo(Type type)
    {
        TypeName = type.GetTypeName();
        BaseType = type.GetTypeBase();

        if (type.IsByRef)
        {
            Type baseType = type.GetElementType();
            if(baseType == null)
                throw new NullReferenceException("Failed to get type's element type!");
            IsArray = baseType.IsArray;
        }
        else
        {
            IsArray = type.IsArray;
        }
    }

    public Type BaseType { get; }
    
    public string TypeName { get; }
    
    public bool IsArray { get; }
}