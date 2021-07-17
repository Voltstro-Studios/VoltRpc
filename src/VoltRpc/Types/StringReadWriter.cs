using System.IO;

namespace VoltRpc.Types
{
    /// <summary>
    ///     <see cref="ITypeReadWriter"/> for <see cref="string"/>
    /// </summary>
    public sealed class StringReadWriter : ITypeReadWriter
    {
        /// <inheritdoc/>
        public void Write(BinaryWriter writer, object obj)
        {
            writer.Write((string)obj);
        }

        /// <inheritdoc/>
        public object Read(BinaryReader reader)
        {
            return reader.ReadString();
        }
    }
}