using System.IO;
using VoltRpc.IO;

namespace VoltRpc.Types
{
    /// <summary>
    ///     Interface for reading and writing a <see cref="System.Type"/>
    /// </summary>
    public interface ITypeReadWriter
    {
        /// <summary>
        ///     Write the type
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void Write(BufferedWriter writer, object obj);
        
        /// <summary>
        ///     Read the type
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public object Read(BufferedReader reader);
    }
}