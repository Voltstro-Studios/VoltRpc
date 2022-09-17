using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using VoltRpc.Types;

namespace VoltRpc.Services;

internal static class ServiceHelper
{
    public static ServiceMethod[] GetAllServiceMethods(
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        Type type, TypeReaderWriterManager typeReaderWriterManager)
    {
        MethodInfo[] interfaceMethods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
                
                //Check to make sure the type reader/writer manager has a type reader/writer for the type
                VoltTypeInfo voltTypeInfo = new(parameterType);
                if (typeReaderWriterManager.GetType(voltTypeInfo.TypeName) == null)
                    throw new NoTypeReaderWriterException($"The type reader/writer manager doesn't have a type reader/writer for {voltTypeInfo.TypeName}!");
                
                ServiceMethodParameter parameter = new()
                {
                    IsOut = parameterInfo.IsOut,
                    IsRef = parameterType.IsByRef && parameterInfo.IsOut == false,
                    TypeInfo = voltTypeInfo
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