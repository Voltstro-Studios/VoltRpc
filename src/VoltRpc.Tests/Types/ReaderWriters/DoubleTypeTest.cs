﻿using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class DoubleTypeTest
{
    [Test]
    public void DoubleTest()
    {
        const double test = 123.48;
        Utils.TestTypeReaderWriter(new DoubleReadWriter(), test);
    }
}