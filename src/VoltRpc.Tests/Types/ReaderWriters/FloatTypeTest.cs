using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class FloatTypeTest
{
    [Test]
    public void FloatTest()
    {
        const float test = 80.93f;
        Utils.TestTypeReaderWriter(new FloatReadWriter(), test);
    }
}