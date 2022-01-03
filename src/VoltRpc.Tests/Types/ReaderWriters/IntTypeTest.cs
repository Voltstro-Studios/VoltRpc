using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class IntTypeTest
{
    [Test]
    public void IntTest()
    {
        const int test = 73;
        Utils.TestTypeReaderWriter(new IntReadWriter(), test);
    }
}