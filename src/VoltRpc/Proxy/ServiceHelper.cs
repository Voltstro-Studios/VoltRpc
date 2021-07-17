using System.Reflection;

namespace VoltRpc.Proxy
{
    internal static class ServiceHelper
    {
        public static ServiceMethod[] GetAllServiceMethods<T>()
        {
            MethodInfo[] interfaceMethods = typeof(T).GetMethods();
            ServiceMethod[] serviceMethods = new ServiceMethod[interfaceMethods.Length];

            for (int i = 0; i < interfaceMethods.Length; i++)
            {
                MethodInfo method = interfaceMethods[i];
                
                //Get the method parameters
                ParameterInfo[] methodParameters = method.GetParameters();
                string[] parametersTypeNames = new string[methodParameters.Length];
                for (int x = 0; x < methodParameters.Length; x++)
                    parametersTypeNames[x] = methodParameters[x].ParameterType.FullName;

                serviceMethods[i] = new ServiceMethod
                {
                    MethodName = $"{method.DeclaringType.FullName}.{method.Name}",
                    MethodInfo = method,
                    ParametersTypeNames = parametersTypeNames
                };
            }

            return serviceMethods;
        }
    }
}