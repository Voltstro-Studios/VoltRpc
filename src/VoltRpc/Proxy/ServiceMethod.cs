using System.Reflection;

namespace VoltRpc.Proxy
{
    internal class ServiceMethod
    {
        public string MethodName { get; set; }
        public MethodInfo MethodInfo { get; set; }
        
        public string[] ParametersTypeNames { get; set; }
        
        public bool IsReturnVoid { get; set; }
        public string ReturnTypeName { get; set; }
    }
}