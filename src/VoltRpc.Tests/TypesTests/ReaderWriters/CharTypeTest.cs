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

    [Test]
    public void CharArrayTest()
    {
        char[] messages = {'g', 'L'};
        Utils.TestTypeReaderWriter(new CharArrayReadWriter(), messages);
    }

    [Test]
    public void CharNullArrayTest()
    {
        char[] messages = null;
        Utils.TestTypeReaderWriter(new CharArrayReadWriter(), messages);
    }
}