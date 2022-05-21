using System.Numerics;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Extension.Vectors.Types;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Vector3"/>
/// </summary>
public sealed class Vector3TypeReadWriter : TypeReadWriter<Vector3>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Vector3 value)
    {
        writer.WriteVector3(value);
    }

    /// <inheritdoc />
    public override Vector3 Read(BufferedReader reader)
    {
        return reader.ReadVector3();
    }
}