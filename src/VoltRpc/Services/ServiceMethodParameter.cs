using VoltRpc.Types;

namespace VoltRpc.Services;

internal struct ServiceMethodParameter
{
    public bool IsRef { get; set; }

    public bool IsOut { get; set; }

    public VoltTypeInfo TypeInfo { get; set; }
}