using System.Collections.Generic;

namespace VoltRpc.Types
{
    internal class TypeReaderWriterManager
    {
        public TypeReaderWriterManager()
        {
            typeReadersWriters = new Dictionary<string, ITypeReadWriter>();
            AddType(typeof(string).FullName, new StringReadWriter());
        }
        
        private readonly Dictionary<string, ITypeReadWriter> typeReadersWriters;

        public void AddType(string type, ITypeReadWriter typeReadWriter)
        {
            typeReadersWriters.Add(type, typeReadWriter);
        }

        public ITypeReadWriter GetType(string type)
        {
            return !typeReadersWriters.ContainsKey(type) ? null : typeReadersWriters[type];
        }
    }
}