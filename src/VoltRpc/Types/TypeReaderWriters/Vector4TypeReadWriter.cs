using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Vector4"/>
/// </summary>
internal sealed class Vector4TypeReadWriter : TypeReadWriter<Vector4>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Vector4 value)
    {
        writer.WriteVector4(value);
    }

    /// <inheritdoc />
    public override Vector4 Read(BufferedReader reader)
    {
        return reader.ReadVector4();
    }
}