using System.Reflection;

namespace VoltRpc.Proxy
{
    internal static class ServiceHelper
    {
        public static ServiceMethod[] GetAllServiceMethods<T>()
        {
            MethodInfo[] serviceMethods = typeof(T).GetMethods();
            ServiceMethod[] methods = new ServiceMethod[serviceMethods.Length];

            for (int i = 0; i < serviceMethods.Length; i++)
            {
                methods[i] = new ServiceMethod
                {
                    MethodName = $"{serviceMethods[i].DeclaringType.FullName}.{serviceMethods[i].Name}",
                    MethodInfo = serviceMethods[i]
                };
            }

            return methods;
        }
    }
}