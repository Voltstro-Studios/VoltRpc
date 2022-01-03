﻿using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class ShortTypeTest
{
    [Test]
    public void ShortTest()
    {
        const short test = short.MaxValue;
        Utils.TestTypeReaderWriter(new ShortReadWriter(), test);
    }
}