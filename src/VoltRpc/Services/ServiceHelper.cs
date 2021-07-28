using System;
using System.Reflection;

namespace VoltRpc.Services
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
                bool containsRefOrOutParameter = false;
                int refOrOutParameterCount = 0;
                ParameterInfo[] methodParameters = method.GetParameters();
                ServiceMethodParameter[] parameters = new ServiceMethodParameter[methodParameters.Length];
                for (int x = 0; x < methodParameters.Length; x++)
                {
                    ParameterInfo parameterInfo = methodParameters[x];
                    Type parameterType = parameterInfo.ParameterType;
                    ServiceMethodParameter parameter = new ServiceMethodParameter
                    {
                        IsOut = parameterInfo.IsOut,
                        IsRef = parameterType.IsByRef && parameterInfo.IsOut == false,
                        ParameterTypeName = parameterType.FullName
                    };
                    parameters[x] = parameter;
                    if (parameter.IsOut || parameter.IsRef)
                    {
                        containsRefOrOutParameter = true;
                        refOrOutParameterCount++;
                    }
                }

                serviceMethods[i] = new ServiceMethod
                {
                    MethodName = $"{method.DeclaringType.FullName}.{method.Name}",
                    MethodInfo = method,
                    ContainsRefOrOutParameters = containsRefOrOutParameter,
                    RefOrOutParameterCount = refOrOutParameterCount,
                    Parameters = parameters,
                    IsReturnVoid = method.ReturnType == typeof(void),
                    ReturnTypeName = method.ReturnType.FullName
                };
            }

            return serviceMethods;
        }
    }
}