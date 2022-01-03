using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class ByteTypeTest
{
    [Test]
    public void ByteTest()
    {
        const byte test = 25;
        Utils.TestTypeReaderWriter(new ByteReadWriter(), test);
    }
}