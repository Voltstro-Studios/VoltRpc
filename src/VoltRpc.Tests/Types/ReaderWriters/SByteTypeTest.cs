using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class SByteTypeTest
{
    [Test]
    public void SByteTest()
    {
        const sbyte test = sbyte.MaxValue;
        Utils.TestTypeReaderWriter(new SByteReadWriter(), test);
    }
}