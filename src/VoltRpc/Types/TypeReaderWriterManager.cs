using System;
using System.Collections.Generic;
using VoltRpc.IO;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Types;

/// <summary>
///     Manger for <see cref="ITypeReadWriter" />s
/// </summary>
public class TypeReaderWriterManager
{
    /// <summary>
    ///     Default <see cref="ITypeReadWriter" /> to be added
    /// </summary>
    internal readonly Dictionary<Type, ITypeReadWriter> DefaultTypeReaderWriters =
        new()
        {
            [typeof(bool)] = new BoolReadWriter(),
            [typeof(byte)] = new ByteReadWriter(),
            [typeof(char)] = new CharReadWriter(),
            [typeof(decimal)] = new DecimalReadWriter(),
            [typeof(double)] = new DoubleReadWriter(),
            [typeof(float)] = new FloatReadWriter(),
            [typeof(int)] = new IntReadWriter(),
            [typeof(long)] = new LongReadWriter(),
            [typeof(sbyte)] = new SByteReadWriter(),
            [typeof(short)] = new ShortReadWriter(),
            [typeof(string)] = new StringReadWriter(),
            [typeof(uint)] = new UIntReadWriter(),
            [typeof(ulong)] = new ULongReadWriter(),
            [typeof(ushort)] = new UShortReadWriter(),
        };

    private readonly Dictionary<string, ITypeReadWriter> typeReadersWriters;

    /// <summary>
    ///     Creates a new <see cref="TypeReaderWriterManager" /> instance
    /// </summary>
    internal TypeReaderWriterManager(bool addDefaults = true)
    {
        typeReadersWriters = new Dictionary<string, ITypeReadWriter>();

        if (!addDefaults)
            return;
        foreach (KeyValuePair<Type, ITypeReadWriter> typeReaderWriter in DefaultTypeReaderWriters)
            AddType(typeReaderWriter.Key, typeReaderWriter.Value);
    }

    /// <summary>
    ///     Adds a <see cref="ITypeReadWriter" />
    ///     <para>
    ///         If the <see cref="System.Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for T</param>
    /// <typeparam name="T">The <see cref="System.Type" /> to add</typeparam>
    public void AddType<T>(ITypeReadWriter typeReadWriter)
    {
        AddType(typeof(T), typeReadWriter);
    }

    /// <summary>
    ///     Adds a <see cref="ITypeReadWriter" />
    ///     <para>
    ///         If the <see cref="System.Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="type">The <see cref="System.Type" /> to add</param>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for <see cref="System.Type" /></param>
    /// <exception cref="NullReferenceException">Thrown if the type is an array, and the base type is null.</exception>
    public void AddType(Type type, ITypeReadWriter typeReadWriter)
    {
        AddType(type.GetTypeName(), typeReadWriter);
    }

    /// <summary>
    ///     Adds a <see cref="ITypeReadWriter" />
    ///     <para>
    ///         If the <see cref="System.Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="typeFullName">The <see cref="System.Type" /> full name to add</param>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for <see cref="System.Type" /></param>
    private void AddType(string typeFullName, ITypeReadWriter typeReadWriter)
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
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <param name="typeFullName">The <see cref="System.Type" /> full name</param>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="System.Type" /></returns>
    public ITypeReadWriter GetType(string typeFullName)
    {
        return !typeReadersWriters.ContainsKey(typeFullName) ? null : typeReadersWriters[typeFullName];
    }

    /// <summary>
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <param name="type">The <see cref="System.Type" /> to get</param>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="System.Type" /></returns>
    public ITypeReadWriter GetType(Type type)
    {
        return GetType(type.FullName);
    }

    /// <summary>
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <typeparam name="T">The <see cref="System.Type" /> to get</typeparam>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="System.Type" /></returns>
    public ITypeReadWriter GetType<T>()
    {
        return GetType(typeof(T));
    }

    internal static object Read(BufferedReader reader, ITypeReadWriter readWriter, VoltTypeInfo type)
    {
        if (type.IsArray)
        {
            //Read size
            int size = reader.ReadInt();

            //Create array
            Array array = Array.CreateInstance(type.BaseType, size);
            for (int i = 0; i < size; i++)
            {
                array.SetValue(readWriter.Read(reader), i);
            }

            return array;
        }

        return readWriter.Read(reader);
    }
    
    internal static void Write(BufferedWriter writer, ITypeReadWriter readWriter, VoltTypeInfo type, object value)
    {
        //If it is an array, write the size first
        if (type.IsArray)
        {
            Array array = (Array) value;

            int length = array.Length;
            writer.WriteInt(array.Length);
            
            if(length <= 0)
                return;

            for (int i = 0; i < length; i++)
            {
                object obj = array.GetValue(i);
                readWriter.Write(writer, obj);
            }
            return;
        }
        
        readWriter.Write(writer, value);
    }
}