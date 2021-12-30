# Types

The default provided types include:

- [`Bool`](xref:System.Boolean)
- [`Byte`](xref:System.Byte)
- [`Char`](xref:System.Char)
- [`Decimal`](xref:System.Decimal)
- [`Double`](xref:System.Double)
- [`Float`](xref:System.Single)
- [`Int`](xref:System.Int32)
- [`Long`](xref:System.Int64)
- [`SByte`](xref:System.SByte)
- [`Short`](xref:System.Int16)
- [`String`](xref:System.String)
- [`UInt`](xref:System.UInt32)
- [`ULong`](xref:System.UInt64)
- [`UShort`](xref:System.UInt16)
    - Array types of above listed
    - Null support for any type that can be null

# Custom Types

Custom types can be implemented using a <xref:VoltRpc.Types.ITypeReadWriter>.

For this example we will be using a custom `CustomType` that looks like this:

```csharp
public struct CustomType
{
    public int UserId { get; set; }

    public string Message { get; set; }
}
```

To create a <xref:VoltRpc.Types.ITypeReadWriter> for `CustomType`, you would do:

```csharp
using VoltRpc.IO;
using VoltRpc.Types;

public class CustomTypeReadWriter : ITypeReadWriter
{
    public void Write(BufferedWriter writer, object obj)
    {
        CustomType customType = (CustomType) obj;
        writer.WriteInt(customType.UserId);
        writer.WriteString(customType.Message);
    }

    public object Read(BufferedReader reader)
    {
        return new CustomType
        {
            UserId = reader.ReadInt(),
            Message = reader.ReadString()
        };
    }
}
```

You can read and write any type supported by <xref:VoltRpc.IO.BufferedWriter> and <xref:VoltRpc.IO.BufferedReader>.

To let the client and host know about this type, you will need to call their respected <xref:VoltRpc.Types.TypeReaderWriterManager>.AddType().

For example:

```csharp
client.TypeReaderWriterManager.AddType<CustomType>(new CustomTypeReadWriter());
host.TypeReaderWriterManager.AddType<CustomType>(new CustomTypeReadWriter());
```