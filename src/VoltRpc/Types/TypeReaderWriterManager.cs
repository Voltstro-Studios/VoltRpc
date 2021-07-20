using System;
using System.Collections.Generic;

namespace VoltRpc.Types
{
    /// <summary>
    ///     Manger for <see cref="ITypeReadWriter"/>s
    /// </summary>
    public class TypeReaderWriterManager
    {
        /// <summary>
        ///     Creates a new <see cref="TypeReaderWriterManager"/> instance
        /// </summary>
        internal TypeReaderWriterManager()
        {
            typeReadersWriters = new Dictionary<string, ITypeReadWriter>();
            
            AddType<string>(new StringReadWriter());
        }
        
        private readonly Dictionary<string, ITypeReadWriter> typeReadersWriters;

        /// <summary>
        ///     Adds a <see cref="ITypeReadWriter"/>
        ///     <para>
        ///         If the <see cref="Type"/> has already been added, it will be overriden
        ///     </para>
        /// </summary>
        /// <param name="typeReadWriter">The <see cref="ITypeReadWriter"/> for T</param>
        /// <typeparam name="T">The <see cref="Type"/> to add</typeparam>
        public void AddType<T>(ITypeReadWriter typeReadWriter)
        {
            AddType(typeof(T), typeReadWriter);
        }
        
        /// <summary>
        ///     Adds a <see cref="ITypeReadWriter"/>
        ///     <para>
        ///         If the <see cref="Type"/> has already been added, it will be overriden
        ///     </para>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to add</param>
        /// <param name="typeReadWriter">The <see cref="ITypeReadWriter"/> for <see cref="Type"/></param>
        public void AddType(Type type, ITypeReadWriter typeReadWriter)
        {
            AddType(type.FullName, typeReadWriter);
        }
        
        /// <summary>
        ///     Adds a <see cref="ITypeReadWriter"/>
        ///     <para>
        ///         If the <see cref="Type"/> has already been added, it will be overriden
        ///     </para>
        /// </summary>
        /// <param name="typeFullName">The <see cref="Type"/> full name to add</param>
        /// <param name="typeReadWriter">The <see cref="ITypeReadWriter"/> for <see cref="Type"/></param>
        public void AddType(string typeFullName, ITypeReadWriter typeReadWriter)
        {
            //If it exists already then replace it
            if (typeReadersWriters.ContainsKey(typeFullName))
            {
                typeReadersWriters[typeFullName] = typeReadWriter;
                return;
            }
            
            typeReadersWriters.Add(typeFullName, typeReadWriter);
        }

        /// <summary>
        ///     Gets a <see cref="ITypeReadWriter"/>
        /// </summary>
        /// <param name="typeFullName">The <see cref="Type"/> full name</param>
        /// <returns>Will return null if <see cref="ITypeReadWriter"/> hasn't been added for <see cref="Type"/></returns>
        public ITypeReadWriter GetType(string typeFullName)
        {
            return !typeReadersWriters.ContainsKey(typeFullName) ? null : typeReadersWriters[typeFullName];
        }
    }
}