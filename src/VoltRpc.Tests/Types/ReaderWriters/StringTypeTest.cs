using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class StringTypeTest
{
    [Test]
    public void StringTest()
    {
        const string test = "Hello World!";
        Utils.TestTypeReaderWriter(new StringReadWriter(), test);
    }

    [Test]
    public void StringNullTest()
    {
        const string test = null;
        Utils.TestTypeReaderWriter(new StringReadWriter(), test);
    }
}