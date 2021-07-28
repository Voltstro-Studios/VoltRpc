using System.Reflection;

namespace VoltRpc.Services
{
    internal class ServiceMethod
    {
        public string MethodName { get; set; }
        public MethodInfo MethodInfo { get; set; }

        public bool ContainsRefOrOutParameters { get; set; }
        public int RefOrOutParameterCount { get; set; }
        public ServiceMethodParameter[] Parameters { get; set; }

        public bool IsReturnVoid { get; set; }
        public string ReturnTypeName { get; set; }
    }
}