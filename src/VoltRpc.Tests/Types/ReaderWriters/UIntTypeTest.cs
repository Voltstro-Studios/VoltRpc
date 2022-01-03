using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class UIntTypeTest
{
    [Test]
    public void UIntTest()
    {
        const uint test = 73;
        Utils.TestTypeReaderWriter(new UIntReadWriter(), test);
    }
}