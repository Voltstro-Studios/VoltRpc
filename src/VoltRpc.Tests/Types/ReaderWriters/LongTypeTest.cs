using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class LongTypeTest
{
    [Test]
    public void LongTest()
    {
        const long test = long.MaxValue;
        Utils.TestTypeReaderWriter(new LongReadWriter(), test);
    }
}