using System;
using System.Globalization;
using System.Numerics;
using Spectre.Console;
using VoltRpc.Demo.Shared;

namespace VoltRpc.Demo.Host;

public class TestImp : ITest
{
    private readonly Random random;
    
    public TestImp()
    {
        random = new Random();
    }
    
    public void BasicTest()
    {
        AnsiConsole.Write(CreateRule("Method Void"));
    }

    public void ParmTest(string message, float num)
    {
        AnsiConsole.Write(CreateRule("Method Void Parameter [[String Number]]"));
        
        Table table = new();
        table.AddColumn("Message");
        table.AddColumn("Number");

        table.AddRow(message);
        table.AddRow(num.ToString(CultureInfo.InvariantCulture));
        
        AnsiConsole.Write(table);
    }

    public string ReturnTest()
    {
        AnsiConsole.Write(CreateRule("Method Return String"));
        
        return "Hello Back!";
    }

    public void ArrayTest(string[] array)
    {
        AnsiConsole.Write(CreateRule("Method Void Parameter [[string array]]"));
        AnsiConsole.WriteLine($"Array: {string.Join(", ", array)}");
    }

    public void RefTest(ref string refTest)
    {
        AnsiConsole.Write(CreateRule("Method Void Parameter [[ref string]]"));
        AnsiConsole.WriteLine($"Got ref of {refTest}");
        refTest = "Hello Back";
    }

    public byte RefReturnTest(ref uint refTest)
    {
        AnsiConsole.Write(CreateRule("Method Return [[byte]] Parameter [[ref uint]]"));
        AnsiConsole.WriteLine($"Got ref of {refTest}");
        refTest = 76;
        return 128;
    }

    public void OutTest(out string outTest)
    {
        AnsiConsole.Write(CreateRule("Method Void Parameter [[out string]]"));
        outTest = "Hello Out!";
    }

    public void CustomTypeTest(CustomType customType)
    {
        AnsiConsole.Write(CreateRule("Method Void Parameter [[CustomType]]"));
        
        Table table = new();
        table.AddColumn("Floaty");
        table.AddColumn("Message");

        table.AddRow(customType.Floaty.ToString(CultureInfo.InvariantCulture), customType.Message);

        AnsiConsole.Write(table);
    }

    public CustomType CustomTypeReturnTest()
    {
        AnsiConsole.Write(CreateRule("Method Return [[CustomType]]"));
        
        return new CustomType
        {
            Floaty = 69.420f,
            Message = "HaHa Reddit big chungus wholesome 100 keanu reeves"
        };
    }

    public CustomTypeArrays CustomTypeArraysSmall()
    {
        AnsiConsole.Write(CreateRule("Method Return [[CustomTypeArrays]]"));
        
        byte[] data = new byte[1920 * 1080];
        random.NextBytes(data);
        return new CustomTypeArrays
        {
            LargeArray = data
        };
    }

    public Vector3 Vector3TypeReturnTest()
    {
        AnsiConsole.Write(CreateRule("Method Return [[Vector3]]"));
        return new Vector3(2.5f, 128.32f, 76.1278f);
    }

    private static Rule CreateRule(string title)
    {
        Rule rule = new(title)
        {
            Alignment = Justify.Left
        };
        return rule;
    }
}