using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class ByteTypeTest
{
    [Test]
    public void ByteTest()
    {
        const byte test = 25;
        Utils.TestTypeReaderWriter(new ByteReadWriter(), test);
    }

    [Test]
    public void ByteArrayTest()
    {
        byte[] messages = {13, 40};
        Utils.TestTypeReaderWriter(new ByteArrayReadWriter(), messages);
    }

    [Test]
    public void ByteNullArrayTest()
    {
        byte[] messages = null;
        Utils.TestTypeReaderWriter(new ByteArrayReadWriter(), messages);
    }
}