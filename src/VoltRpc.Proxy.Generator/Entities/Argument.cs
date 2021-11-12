namespace VoltRpc.Proxy.Generator.Entities;

public struct Argument
{
    public string type;
    public string name;
    public string trailing;

    public bool isref;
    public bool isout;
}