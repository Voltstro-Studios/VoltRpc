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
            string stringObj = (string) obj;
            if (stringObj == null)
            {
                writer.Write((byte)0);
                return;
            }
            
            writer.Write((byte)1);
            writer.Write(stringObj);
        }

        /// <inheritdoc/>
        public object Read(BinaryReader reader)
        {
            byte isNull = reader.ReadByte();
            return isNull == 0 ? null : reader.ReadString();
        }
    }
}