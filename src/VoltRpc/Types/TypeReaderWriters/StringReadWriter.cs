using VoltRpc.IO;

namespace VoltRpc.Types.TypeReaderWriters
{
    /// <summary>
    ///     <see cref="ITypeReadWriter"/> for <see cref="string"/>
    /// </summary>
    internal sealed class StringReadWriter : ITypeReadWriter
    {
        /// <inheritdoc/>
        public void Write(BufferedWriter writer, object obj)
        {
            string stringObj = (string) obj;
            writer.WriteString(stringObj);
        }

        /// <inheritdoc/>
        public object Read(BufferedReader reader)
        {
            return reader.ReadString();
        }
    }
}