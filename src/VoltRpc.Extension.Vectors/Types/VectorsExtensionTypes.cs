using VoltRpc.Types;

namespace VoltRpc.Extension.Vectors.Types;

/// <summary>
///     Vectors extension types manager
/// </summary>
public static class VectorsExtensionTypes
{
    /// <summary>
    ///     Adds all <see cref="TypeReadWriter{T}"/> that VoltRpc.Extension.Vectors supports
    /// </summary>
    /// <param name="readerWriterManager"></param>
    public static void InstallVectorsExtension(this TypeReaderWriterManager readerWriterManager)
    {
        readerWriterManager.AddType(new Matrix3X2TypeReadWriter());
        readerWriterManager.AddType(new Matrix4X4TypeReadWriter());
        readerWriterManager.AddType(new PlaneTypeReadWriter());
        readerWriterManager.AddType(new QuaternionTypeReadWriter());
        readerWriterManager.AddType(new Vector2TypeReadWriter());
        readerWriterManager.AddType(new Vector3TypeReadWriter());
        readerWriterManager.AddType(new Vector4TypeReadWriter());
    }
}