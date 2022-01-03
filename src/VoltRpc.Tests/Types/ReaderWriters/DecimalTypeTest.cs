using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class DecimalTypeTest
{
    [Test]
    public void DecimalTest()
    {
        const decimal test = 3;
        Utils.TestTypeReaderWriter(new DecimalReadWriter(), test);
    }
}