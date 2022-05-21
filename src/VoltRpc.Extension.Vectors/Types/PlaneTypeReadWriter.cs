using System.Numerics;
using VoltRpc.IO;
using VoltRpc.Types;

namespace VoltRpc.Extension.Vectors.Types;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Plane"/>
/// </summary>
public sealed class PlaneTypeReadWriter : TypeReadWriter<Plane>
{
    /// <inheritdoc />
    public override void Write(BufferedWriter writer, Plane value)
    {
        writer.WritePlane(value);
    }

    /// <inheritdoc />
    public override Plane Read(BufferedReader reader)
    {
        return reader.ReadPlane();
    }
}