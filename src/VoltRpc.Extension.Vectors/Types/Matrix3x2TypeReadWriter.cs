using System.Numerics;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Extension.Vectors.Types;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Matrix3x2"/>
/// </summary>
public sealed class Matrix3X2TypeReadWriter : TypeReadWriter<Matrix3x2>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Matrix3x2 value)
    {
        writer.WriteMatrix3X2(value);
    }

    /// <inheritdoc />
    public override Matrix3x2 Read(BufferedReader reader)
    {
        return reader.ReadMatrix3X2();
    }
}