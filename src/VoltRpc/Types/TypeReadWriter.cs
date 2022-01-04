using VoltRpc.IO;

namespace VoltRpc.Types;

/// <summary>
///     Interface for reading and writing a type.
///     <para><see cref="Read"/> should read exactly how it is written to the <see cref="BufferedWriter"/> in <see cref="Write"/>.</para>
/// </summary>
/// <typeparam name="T">The <see cref="System.Type"/> to read and write</typeparam>
public abstract class TypeReadWriter<T> : ITypeReadWriter
{
    /// <summary>
    ///     Called when the type needs to be written
    /// </summary>
    /// <param name="writer">The <see cref="BufferedWriter"/> to write to</param>
    /// <param name="value">The value that needs to be written to the <see cref="BufferedWriter"/></param>
    public abstract void Write(BufferedWriter writer, T value);

    void ITypeReadWriter.Write(BufferedWriter writer, object obj)
    {
        Write(writer, (T)obj);
    }

    /// <summary>
    ///     Called when the type needs to be read
    /// </summary>
    /// <param name="reader">The <see cref="BufferedReader"/> to read from</param>
    /// <returns>Return the read value from the <see cref="BufferedReader"/></returns>
    public abstract T Read(BufferedReader reader);
    
    object ITypeReadWriter.Read(BufferedReader reader)
    {
        return Read(reader);
    }
}