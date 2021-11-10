using System;
using System.Collections.Generic;
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
            [typeof(bool[])] = new BoolArrayReadWriter(),
            [typeof(byte)] = new ByteReadWriter(),
            [typeof(byte[])] = new ByteArrayReadWriter(),
            [typeof(char)] = new CharReadWriter(),
            [typeof(char[])] = new CharArrayReadWriter(),
            [typeof(decimal)] = new DecimalReadWriter(),
            [typeof(decimal[])] = new DecimalReadWriter(),
            [typeof(double)] = new DoubleReadWriter(),
            [typeof(double[])] = new DoubleArrayReadWriter(),
            [typeof(float)] = new FloatReadWriter(),
            [typeof(float[])] = new FloatArrayReadWriter(),
            [typeof(int)] = new IntReadWriter(),
            [typeof(int[])] = new IntArrayReadWriter(),
            [typeof(long)] = new LongReadWriter(),
            [typeof(long[])] = new LongArrayReadWriter(),
            [typeof(sbyte)] = new SByteReadWriter(),
            [typeof(sbyte[])] = new SByteArrayReadWriter(),
            [typeof(short)] = new ShortReadWriter(),
            [typeof(short[])] = new ShortArrayReadWriter(),
            [typeof(string)] = new StringReadWriter(),
            [typeof(string[])] = new StringArrayReadWriter(),
            [typeof(uint)] = new UIntReadWriter(),
            [typeof(uint[])] = new UIntArrayReadWriter(),
            [typeof(ulong)] = new ULongReadWriter(),
            [typeof(ulong[])] = new ULongArrayReadWriter(),
            [typeof(ushort)] = new UShortReadWriter(),
            [typeof(ushort[])] = new UShortArrayReadWriter()
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
    ///         If the <see cref="Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for T</param>
    /// <typeparam name="T">The <see cref="Type" /> to add</typeparam>
    public void AddType<T>(ITypeReadWriter typeReadWriter)
    {
        AddType(typeof(T), typeReadWriter);
    }

    /// <summary>
    ///     Adds a <see cref="ITypeReadWriter" />
    ///     <para>
    ///         If the <see cref="Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to add</param>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for <see cref="Type" /></param>
    public void AddType(Type type, ITypeReadWriter typeReadWriter)
    {
        AddType(type.FullName, typeReadWriter);
    }

    /// <summary>
    ///     Adds a <see cref="ITypeReadWriter" />
    ///     <para>
    ///         If the <see cref="Type" /> has already been added, it will be overriden
    ///     </para>
    /// </summary>
    /// <param name="typeFullName">The <see cref="Type" /> full name to add</param>
    /// <param name="typeReadWriter">The <see cref="ITypeReadWriter" /> for <see cref="Type" /></param>
    public void AddType(string typeFullName, ITypeReadWriter typeReadWriter)
    {
        //If it exists already then replace it
        if (typeReadersWriters.ContainsKey(typeFullName))
        {
            typeReadersWriters[typeFullName] = typeReadWriter;
            typeReadersWriters[$"{typeFullName}&"] = typeReadWriter;
            return;
        }

        typeReadersWriters.Add(typeFullName, typeReadWriter);
        typeReadersWriters.Add($"{typeFullName}&", typeReadWriter);
    }

    /// <summary>
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <param name="typeFullName">The <see cref="Type" /> full name</param>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="Type" /></returns>
    public ITypeReadWriter GetType(string typeFullName)
    {
        return !typeReadersWriters.ContainsKey(typeFullName) ? null : typeReadersWriters[typeFullName];
    }

    /// <summary>
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <param name="type">The <see cref="Type" /> to get</param>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="Type" /></returns>
    public ITypeReadWriter GetType(Type type)
    {
        return GetType(type.FullName);
    }

    /// <summary>
    ///     Gets a <see cref="ITypeReadWriter" />
    /// </summary>
    /// <typeparam name="T">The <see cref="Type" /> to get</typeparam>
    /// <returns>Will return null if <see cref="ITypeReadWriter" /> hasn't been added for <see cref="Type" /></returns>
    public ITypeReadWriter GetType<T>()
    {
        return GetType(typeof(T));
    }
}