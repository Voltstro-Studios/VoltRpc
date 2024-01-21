using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Vector2"/>
/// </summary>
public sealed class Vector2TypeReadWriter : TypeReadWriter<Vector2>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Vector2 value)
    {
        writer.WriteVector2(value);
    }

    /// <inheritdoc />
    public override Vector2 Read(BufferedReader reader)
    {
        return reader.ReadVector2();
    }
}