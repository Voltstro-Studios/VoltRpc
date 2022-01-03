using System;
using System.Collections.Generic;
using NUnit.Framework;
using VoltRpc.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types;

public class TypeReaderWriterManagerTests
{
    private TypeReaderWriterManager readerWriterManager;

    [OneTimeSetUp]
    public void Setup()
    {
        readerWriterManager = new TypeReaderWriterManager(false);
    }

    [Test]
    public void DefaultTypes()
    {
        TypeReaderWriterManager manager = new();
        foreach (KeyValuePair<Type, ITypeReadWriter> defaultTypeReaderWriter in manager.DefaultTypeReaderWriters)
        {
            ITypeReadWriter readWriter = manager.GetType(defaultTypeReaderWriter.Key);
            Assert.NotNull(readWriter);
            Assert.AreEqual(defaultTypeReaderWriter.Value, readWriter);
        }
    }

    [Test]
    public void AddTypeTest()
    {
        BoolReadWriter boolReadWriter = new();
        readerWriterManager.AddType<bool>(boolReadWriter);

        ITypeReadWriter readWriter = readerWriterManager.GetType<bool>();
        Assert.IsNotNull(readWriter);
        Assert.AreEqual(boolReadWriter, readWriter);
    }

    [Test]
    public void OverrideTypeTest()
    {
        CustomBool boolReadWriter = new();
        readerWriterManager.AddType<bool>(new BoolReadWriter());
        readerWriterManager.AddType<bool>(boolReadWriter);

        ITypeReadWriter readWriter = readerWriterManager.GetType<bool>();
        Assert.IsNotNull(readWriter);
        Assert.AreEqual(boolReadWriter, readWriter);
    }

    private class CustomBool : ITypeReadWriter
    {
        public void Write(BufferedWriter writer, object obj)
        {
        }

        public object Read(BufferedReader reader)
        {
            return null;
        }
    }
}