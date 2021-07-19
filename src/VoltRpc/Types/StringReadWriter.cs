using VoltRpc.IO;

namespace VoltRpc.Types
{
    /// <summary>
    ///     <see cref="ITypeReadWriter"/> for <see cref="string"/>
    /// </summary>
    public sealed class StringReadWriter : ITypeReadWriter
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