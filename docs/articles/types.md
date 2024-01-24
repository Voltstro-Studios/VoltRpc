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
- [`UInt`](xref:System.UInt32)
- [`ULong`](xref:System.UInt64)
- [`UShort`](xref:System.UInt16)
- [`String`](xref:System.String)
- [`DateTime`](xref:System.DateTime)
- [`TimeSpan`](xref:System.TimeSpan)
- [`Uri`](xref:System.Uri)
- [`Vector2`](xref:System.Numerics.Vector2)
- [`Vector3`](xref:System.Numerics.Vector3)
- [`Vector4`](xref:System.Numerics.Vector4)
- [`Quaternion`](xref:System.Numerics.Quaternion)
- [`Plane`](xref:System.Numerics.Plane)
- [`Matrix3x2`](xref:System.Numerics.Matrix3x2)
- [`Matrix4x4`](xref:System.Numerics.Matrix4x4)

VoltRpc will automatically handle arrays for any type.

# Custom Types

Custom types can be implemented using a <xref:VoltRpc.Types.TypeReadWriter`1>.

For this example we will be using a custom `CustomType` that looks like this:

```csharp
public struct CustomType
{
    public int UserId { get; set; }

    public string Message { get; set; }
}
```

To create a <xref:VoltRpc.Types.TypeReadWriter`1> for `CustomType`, you would do:

```csharp
using VoltRpc.IO;
using VoltRpc.Types;

public class CustomTypeReadWriter : TypeReadWriter<CustomType>
{
    public void Write(BufferedWriter writer, CustomType customType)
    {
        writer.WriteInt(customType.UserId);
        writer.WriteString(customType.Message);
    }

    public CustomType Read(BufferedReader reader)
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

To let the client and host know about this type, you will need to call their respected [TypeReaderWriterManager.AddType](xref:VoltRpc.Types.TypeReaderWriterManager.AddType*).

For example:

```csharp
client.TypeReaderWriterManager.AddType<CustomType>(new CustomTypeReadWriter());
host.TypeReaderWriterManager.AddType<CustomType>(new CustomTypeReadWriter());
```