#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoltRpc.Proxy.Generator.Entities;

internal readonly struct Method
{
    internal Method(string interfaceFullName, string methodName, string? returnTypeFullName, List<Argument>? arguments)
    {
        InterfaceFullName = interfaceFullName;
        MethodName = methodName;
        ReturnTypeFullName = returnTypeFullName;
        Arguments = arguments;
    }
    
    private string InterfaceFullName { get; }
    private string MethodName { get; }
    private string? ReturnTypeFullName { get; }
    
    private List<Argument>? Arguments { get; }

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append("public ");
        builder.Append(ReturnTypeFullName == null ? "void " : $"{ReturnTypeFullName} ");
        builder.Append($"{MethodName}({(Arguments == null ? string.Empty : string.Join(", ", Arguments))})");
        builder.Append("\n");
        builder.Append("{");
        builder.Append("\n\t");
        if (ReturnTypeFullName != null || (Arguments != null && Arguments.Any(x => x.IsOut || x.IsRef)))
            builder.Append("object[] returnObjects = ");
        builder.Append($"client.InvokeMethod(\"{InterfaceFullName}.{MethodName}\"");
        if (Arguments != null && Arguments.Any(x => !x.IsOut))
        {
            builder.Append(", new object[] {");
            IEnumerable<Argument> argumentsToInclude = Arguments.Where(x => !x.IsOut);
            string argumentsRendered = string.Join(", ", argumentsToInclude.Select(x => x.ArgumentName));
            builder.Append(argumentsRendered);
            builder.Append("}");
        }
        builder.Append(");\n");

        int index = 0;
        if (ReturnTypeFullName != null)
            index = 1;

        if (Arguments != null)
            foreach (Argument argument in Arguments)
            {
                if (!argument.IsOut && !argument.IsRef)
                    continue;

                builder.Append($"\t{argument.ArgumentName} = ({argument.ArgumentTypeFullName})returnObjects[{index}];");
                builder.Append("\n");

                index++;
            }

        if (ReturnTypeFullName != null)
        {
            builder.Append($"\treturn ({ReturnTypeFullName})returnObjects[0];");
            builder.Append("\n");
        }

        builder.Append("}");
        return builder.ToString();
    }
}