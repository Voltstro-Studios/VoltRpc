using System.Text;

namespace VoltRpc.Proxy.Generator.Entities;

internal readonly struct Argument
{
    internal Argument(string argumentName, string argumentTypeFullName, bool isArray, bool isRef, bool isOut)
    {
        ArgumentName = argumentName;
        ArgumentTypeFullName = argumentTypeFullName;
        IsArray = isArray;
        IsRef = isRef;
        IsOut = isOut;
    }
    
    public string ArgumentName { get; }
    public string ArgumentTypeFullName { get; }

    public bool IsArray { get; }
    public bool IsRef { get; }
    public bool IsOut { get; }

    public override string ToString()
    {
        StringBuilder builder = new();
        if (IsRef)
            builder.Append("ref ");
        if (IsOut)
            builder.Append("out ");
        builder.Append($"{ArgumentTypeFullName} {ArgumentName}");
        return builder.ToString();
    }
}