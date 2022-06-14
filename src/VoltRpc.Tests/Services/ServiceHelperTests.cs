using NUnit.Framework;
using VoltRpc.Services;
using VoltRpc.Tests.TestObjects.Interfaces;
using VoltRpc.Types;

namespace VoltRpc.Tests.Services;

public class ServiceHelperTests
{
    private TypeReaderWriterManager typeReaderWriterManager;
    
    [OneTimeSetUp]
    public void Setup()
    {
        typeReaderWriterManager = new TypeReaderWriterManager();
    }
    
    [Test]
    public void ServiceBasicInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IBasicInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic", true, 0, 0);
    }

    [Test]
    public void ServiceBasicMultipleInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IBasicMultipleInterface), typeReaderWriterManager);
        Assert.AreEqual(2, methods.Length);

        //Method 1
        ServiceMethod method1 = methods[0];
        CheckMethod(method1, "VoltRpc.Tests.TestObjects.Interfaces.IBasicMultipleInterface.Basic1", true, 0, 0);

        //Method 2
        ServiceMethod method2 = methods[1];
        CheckMethod(method2, "VoltRpc.Tests.TestObjects.Interfaces.IBasicMultipleInterface.Basic2", true, 0, 0);
    }

    [Test]
    public void ServiceReturnBasicInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IReturnInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IReturnInterface.ReturnBasic", false, 0, 0);
        CheckReturn(method, "System.Int32", false);
    }

    [Test]
    public void ServiceReturnArrayInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IReturnArrayInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IReturnArrayInterface.ReturnArray", false, 0, 0);
        CheckReturn(method, "System.Int32", true);
    }

    [Test]
    public void ServiceParamBasicInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IParameterBasicInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IParameterBasicInterface.BasicParam", true, 0, 1);
        CheckParameter(method.Parameters[0], "System.Int32", false, false, false);
    }

    [Test]
    public void ServiceParamArrayInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IParameterArrayInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IParameterArrayInterface.ArrayParam", true, 0, 1);
        CheckParameter(method.Parameters[0], "System.String", false, false, true);
    }

    [Test]
    public void ServiceRefBasicInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IRefBasicInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IRefBasicInterface.RefBasic", true, 1, 1);
        CheckParameter(method.Parameters[0], "System.Int32", false, true, false);
    }

    [Test]
    public void ServiceRefArrayInterfaceTest()
    {
        ServiceMethod[] methods = ServiceHelper.GetAllServiceMethods(typeof(IRefArrayInterface), typeReaderWriterManager);
        Assert.AreEqual(1, methods.Length);

        ServiceMethod method = methods[0];
        CheckMethod(method, "VoltRpc.Tests.TestObjects.Interfaces.IRefArrayInterface.RefArray", true, 1, 1);
        CheckParameter(method.Parameters[0], "System.String", false, true, true);
    }

    [Test]
    public void ServiceNoTypeReaderWriterTest()
    {
        //Use a type reader writer with no types
        TypeReaderWriterManager testTypeReaderWriterManager = new(false);

        Assert.Throws<NoTypeReaderWriterException>(() =>
            ServiceHelper.GetAllServiceMethods(typeof(IParameterBasicInterface), testTypeReaderWriterManager));
    }

    private void CheckMethod(ServiceMethod method, string methodName, bool methodReturnVoid, int refOrOutParamCount,
        int parameterCount)
    {
        StringAssert.AreEqualIgnoringCase(methodName, method.MethodName);
        Assert.AreEqual(methodReturnVoid, method.IsReturnVoid);
        Assert.AreEqual(refOrOutParamCount != 0, method.ContainsRefOrOutParameters);
        Assert.AreEqual(refOrOutParamCount, method.RefOrOutParameterCount);
        Assert.AreEqual(parameterCount, method.Parameters.Length);
    }

    private void CheckReturn(ServiceMethod method, string returnTypeName, bool isArray)
    {
        StringAssert.AreEqualIgnoringCase(method.ReturnType.TypeName, returnTypeName);
        Assert.AreEqual(isArray, method.ReturnType.IsArray);
    }

    private void CheckParameter(ServiceMethodParameter parameter, string typeFullName, bool isOut, bool isRef,
        bool isArray)
    {
        StringAssert.AreEqualIgnoringCase(typeFullName, parameter.TypeInfo.TypeName);
        Assert.AreEqual(isOut, parameter.IsOut);
        Assert.AreEqual(isRef, parameter.IsRef);
        Assert.AreEqual(isArray, parameter.TypeInfo.IsArray);
    }
}