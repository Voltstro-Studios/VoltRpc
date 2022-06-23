using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Demo.Shared;

/// <summary>
///     A <see cref="TypeReadWriter{T}"/> for <see cref="CustomType"/>
/// </summary>
public class CustomTypeReaderWriter : TypeReadWriter<CustomType>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, CustomType obj)
    {
        writer.WriteFloat(obj.Floaty);
        writer.WriteString(obj.Message);
    }

    /// <inheritdoc />
    public override CustomType Read(BufferedReader reader)
    {
        return new CustomType
        {
            Floaty = reader.ReadFloat(),
            Message = reader.ReadString()
        };
    }
}