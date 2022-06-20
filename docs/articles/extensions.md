# Extensions

VoltRpc has some first-party extensions for it. These are usually for extending the <xref:System.Type> capability of <xref:VoltRpc.IO.BufferedWriter> and <xref:VoltRpc.IO.BufferedReader> as well as providing <xref:VoltRpc.Types.TypeReadWriter`1> for usage by a <xref:VoltRpc.Types.TypeReaderWriterManager>.

# System.Numerics.Vectors Support

The VoltRpc.Extension.Vectors package provides support for almost all types provided by [System.Numerics.Vectors](https://www.nuget.org/packages/System.Numerics.Vectors/).

To install it, you can add it to your project's `csproj` like so:

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc.Extension.Vectors" Version="1.1.0" />
</ItemGroup>
```

Once you have the NuGet package installed, you can use the types by using [`InstallVectorsExtension()`](xref:VoltRpc.Extension.Vectors.Types.VectorsExtensionTypes.InstallVectorsExtension(VoltRpc.Types.TypeReaderWriterManager)) method with the <xref:VoltRpc.Types.TypeReaderWriterManager>.

So for example:

```csharp
VoltRpc.Communication.Client client = new TCPClient(new IPEndPoint(IPAddress.Loopback, 7767));
client.TypeReaderWriterManager.InstallVectorsExtension();
client.AddService<ITest>();
...
```

Both the [client](xref:VoltRpc.Communication.Client.TypeReaderWriterManager) and [host](xref:VoltRpc.Communication.Host.TypeReaderWriterManager) expose their <xref:VoltRpc.Types.TypeReaderWriterManager> using the same field names.

If you wanted to, you can also manually add each type reader/writer.

You can also use the types in your own [custom type readers/writers](types.md#custom-types), using the methods provided in <xref:VoltRpc.Extension.Vectors.BufferedReaderVectorsExtensions> and <xref:VoltRpc.Extension.Vectors.BufferedWriterVectorsExtensions>.

# System.Memory Support

The VoltRpc.Extension.Memory provides support for Span and Memory. It ONLY adds to the <xref:VoltRpc.IO.BufferedReader> and <xref:VoltRpc.IO.BufferedWriter>. It does NOT add type readers/writers! This package also provides a Span way of reading strings.

To install it, you can add it to your project's `csproj` like so:

```xml
<ItemGroup>
    <PackageReference Include="VoltRpc.Extension.Memory" Version="1.1.0" />
</ItemGroup>
```

Once you have the NuGet package installed, you can use the methods in <xref:VoltRpc.Extension.Memory.BufferedReaderMemoryExtensions> and <xref:VoltRpc.Extension.Memory.BufferedWriterMemoryExtensions> in your [custom type readers/writers](types.md#custom-types).