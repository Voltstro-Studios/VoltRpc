using System.Numerics;
using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters;

/// <summary>
///     <see cref="TypeReadWriter{T}"/> for a <see cref="Plane"/>
/// </summary>
internal sealed class PlaneTypeReadWriter : TypeReadWriter<Plane>
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