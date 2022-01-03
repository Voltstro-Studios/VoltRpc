using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class BoolTypeTest
{
    [Test]
    public void BoolTest()
    {
        const bool test = true;
        Utils.TestTypeReaderWriter(new BoolReadWriter(), test);
    }
}