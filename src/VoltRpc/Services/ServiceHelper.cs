using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using VoltRpc.Types;

namespace VoltRpc.Services;

internal static class ServiceHelper
{
#if NET6_0_OR_GREATER
    public static ServiceMethod[] GetAllServiceMethods(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        Type type)
#else
        public static ServiceMethod[] GetAllServiceMethods(Type type)
#endif
    {
        MethodInfo[] interfaceMethods = type.GetMethods();
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
                ServiceMethodParameter parameter = new()
                {
                    IsOut = parameterInfo.IsOut,
                    IsRef = parameterType.IsByRef && parameterInfo.IsOut == false,
                    TypeInfo = new VoltTypeInfo(parameterType)
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
                ReturnType = new VoltTypeInfo(method.ReturnType)
            };
        }

        return serviceMethods;
    }
}