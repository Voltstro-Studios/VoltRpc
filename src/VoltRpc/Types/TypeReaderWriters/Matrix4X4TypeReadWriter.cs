using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Matrix4x4"/>
/// </summary>
internal sealed class Matrix4X4TypeReadWriter : TypeReadWriter<Matrix4x4>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Matrix4x4 value)
    {
        writer.WriteMatrix4X4(value);
    }

    /// <inheritdoc />
    public override Matrix4x4 Read(BufferedReader reader)
    {
        return reader.ReadMatrix4X4();
    }
}