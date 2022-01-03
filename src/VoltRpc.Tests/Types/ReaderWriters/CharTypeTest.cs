using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class CharTypeTest
{
    [Test]
    public void CharTest()
    {
        const char test = 'g';
        Utils.TestTypeReaderWriter(new CharReadWriter(), test);
    }
}