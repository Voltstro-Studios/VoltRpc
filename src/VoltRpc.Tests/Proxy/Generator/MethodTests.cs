using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using VoltRpc.Proxy.Generator.Entities;

namespace VoltRpc.Tests.Proxy.Generator;

public class MethodTests
{
    [Test]
    public void MethodBasicTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic void Basic()\n{\n\tclient.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\");\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, null, null);
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodArgumentTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic void Basic(System.Int32 @value)\n{\n\tclient.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\", new object[] {@value});\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, null, new List<Argument>
        {
            new("value", "System.Int32", false, false, false)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodArgumentReturnTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic System.Int32 Basic(System.Int32 @value)\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\", new object[] {@value});\n\treturn (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, "System.Int32", new List<Argument>
        {
            new("value", "System.Int32", false, false, false)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }

    [Test]
    public void MethodReturnTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic System.Int32 Basic()\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\");\n\treturn (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, "System.Int32", null);
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodOutTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic void Basic(out System.Int32 @value)\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\");\n\t@value = (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, null, new List<Argument>
        {
            new("value", "System.Int32", false, false, true)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodOutReturnTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic System.Int32 Basic(out System.Int32 @value)\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\");\n\t@value = (System.Int32)returnObjects[1];\n\treturn (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, "System.Int32", new List<Argument>
        {
            new("value", "System.Int32", false, false, true)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodRefTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic void Basic(ref System.Int32 @value)\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\", new object[] {@value});\n\t@value = (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, null, new List<Argument>
        {
            new("value", "System.Int32", false, true, false)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
    
    [Test]
    public void MethodRefReturnTest()
    {
        const string methodOutput =
            "/// <inheritdoc />\npublic System.Int32 Basic(ref System.Int32 @value)\n{\n\tobject[] returnObjects = client.InvokeMethod(\"VoltRpc.Tests.ITestInterface.Basic\", new object[] {@value});\n\t@value = (System.Int32)returnObjects[1];\n\treturn (System.Int32)returnObjects[0];\n}";
        Method method = new Method("VoltRpc.Tests.ITestInterface", "Basic", Accessibility.Public, "System.Int32", new List<Argument>
        {
            new("value", "System.Int32", false, true, false)
        });
        StringAssert.AreEqualIgnoringCase(methodOutput, method.ToString());
    }
}