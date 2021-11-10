using System;
using VoltRpc.Demo.Shared;

namespace VoltRpc.Demo.Host;

public class TestImp : ITest
{
    public void BasicTest()
    {
        Console.WriteLine("Basic Test!");
    }

    public void ParmTest(string message, float num)
    {
        if (message == null)
        {
            Console.WriteLine("The message was null");
            return;
        }

        Console.WriteLine(num);
        Console.WriteLine(message);
    }

    public string ReturnTest()
    {
        return "Hello Back!";
    }

    public void ArrayTest(string[] array)
    {
        Console.WriteLine("Got array");
        foreach (string s in array) Console.WriteLine(s);
    }

    public void RefTest(ref string refTest)
    {
        Console.WriteLine($"Ref was {refTest}");
        refTest = "Hello Back";
    }

    public byte RefReturnTest(ref uint refTest)
    {
        refTest = 76;
        return 128;
    }

    public void OutTest(out string outTest)
    {
        Console.WriteLine("Got out test");
        outTest = "Hello Out!";
    }

    public void CustomTypeTest(CustomType customType)
    {
        Console.WriteLine($"Got custom type with values of: {customType.Floaty} {customType.Message}");
    }

    public CustomType CustomTypeReturnTest()
    {
        return new CustomType
        {
            Floaty = 69.420f,
            Message = "HaHa Reddit big chungus wholesome 100 keanu reeves"
        };
    }
}