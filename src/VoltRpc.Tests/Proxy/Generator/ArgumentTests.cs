using System.Collections.Generic;
using NUnit.Framework;
using VoltRpc.Proxy.Generator.Entities;

namespace VoltRpc.Tests.Proxy.Generator;

public class ArgumentTests
{
    [Test]
    public void ArgumentBasicTest()
    {
        Argument argument = new("name", "System.String", false, false, false);
        string renderedArgument = argument.ToString();
        StringAssert.AreEqualIgnoringCase("System.String @name", renderedArgument);
    }

    [Test]
    public void ArgumentRefTest()
    {
        Argument argument = new("name", "System.String", false, true, false);
        string renderedArgument = argument.ToString();
        StringAssert.AreEqualIgnoringCase("ref System.String @name", renderedArgument);
    }

    [Test]
    public void ArgumentOutTest()
    {
        Argument argument = new("name", "System.String", false, false, true);
        string renderedArgument = argument.ToString();
        StringAssert.AreEqualIgnoringCase("out System.String @name", renderedArgument);
    }
    
    [Test]
    public void ArgumentChainedTest()
    {
        List<Argument> arguments = new()
        {
            new Argument("name", "System.String", false, false, false),
            new Argument("value", "System.Int32", false, false, false)
        };

        string renderedArguments = string.Join(", ", arguments);
        StringAssert.AreEqualIgnoringCase("System.String @name, System.Int32 @value", renderedArguments);
    }
}