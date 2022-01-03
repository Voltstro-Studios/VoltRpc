using System.Collections.Generic;

namespace VoltRpc.Proxy.Generator.Entities;

public struct Method
{
    public string interfacename;
    public string interfacenamespace;

    public string returntype;
    public string name;

    public bool returnsvoid;

    public bool anyarrays;
    public List<Argument> arguments;
}