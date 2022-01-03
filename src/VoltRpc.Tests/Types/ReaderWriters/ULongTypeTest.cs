using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class ULongTypeTest
{
    [Test]
    public void ULongTest()
    {
        const ulong test = ulong.MaxValue;
        Utils.TestTypeReaderWriter(new ULongReadWriter(), test);
    }
}