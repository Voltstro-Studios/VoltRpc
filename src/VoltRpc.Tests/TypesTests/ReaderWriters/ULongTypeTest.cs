using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class ULongTypeTest
{
    [Test]
    public void ULongTest()
    {
        const ulong test = ulong.MaxValue;
        Utils.TestTypeReaderWriter(new ULongReadWriter(), test);
    }

    [Test]
    public void ULongArrayTest()
    {
        ulong[] messages = {ulong.MaxValue, 800};
        Utils.TestTypeReaderWriter(new ULongArrayReadWriter(), messages);
    }

    [Test]
    public void ULongNullArrayTest()
    {
        ulong[] messages = null;
        Utils.TestTypeReaderWriter(new ULongArrayReadWriter(), messages);
    }
}