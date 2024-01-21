using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Quaternion"/>
/// </summary>
public sealed class QuaternionTypeReadWriter : TypeReadWriter<Quaternion>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Quaternion value)
    {
        writer.WriteQuaternion(value);
    }

    /// <inheritdoc />
    public override Quaternion Read(BufferedReader reader)
    {
        return reader.ReadQuaternion();
    }
}