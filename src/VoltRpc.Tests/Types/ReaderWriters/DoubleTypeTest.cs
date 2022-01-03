using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class DoubleTypeTest
{
    [Test]
    public void DoubleTest()
    {
        const double test = 123.48;
        Utils.TestTypeReaderWriter(new DoubleReadWriter(), test);
    }

    [Test]
    public void DoubleArrayTest()
    {
        double[] messages = {128.32, 700.4};
        Utils.TestTypeReaderWriter(new DoubleArrayReadWriter(), messages);
    }

    [Test]
    public void DoubleNullArrayTest()
    {
        double[] messages = null;
        Utils.TestTypeReaderWriter(new DoubleArrayReadWriter(), messages);
    }
}