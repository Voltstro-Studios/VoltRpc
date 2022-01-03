using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class UShortTypeTest
{
    [Test]
    public void UShortTest()
    {
        const ushort test = ushort.MaxValue;
        Utils.TestTypeReaderWriter(new UShortReadWriter(), test);
    }
}