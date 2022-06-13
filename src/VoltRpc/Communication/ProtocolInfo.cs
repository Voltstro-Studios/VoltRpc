using System;
using VoltRpc.Types;

namespace VoltRpc.Communication;

internal struct ProtocolInfo
{
    public ProtocolInfo(object value, Type type)
    {
        TypeInfo = new VoltTypeInfo(type);
        Value = value;
    }

    public VoltTypeInfo TypeInfo { get; }

    public object Value { get; }
}