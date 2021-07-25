namespace VoltRpc.Proxy
{
    internal struct Parameter
    {
        public bool IsRef { get; set; }
        
        public bool IsOut { get; set; }
        
        public string ParameterTypeName { get; set; }
    }
}