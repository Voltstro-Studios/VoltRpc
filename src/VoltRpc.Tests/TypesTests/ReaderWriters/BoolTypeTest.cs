using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters;

public class BoolTypeTest
{
    [Test]
    public void BoolTest()
    {
        const bool test = true;
        Utils.TestTypeReaderWriter(new BoolReadWriter(), test);
    }

    [Test]
    public void BoolArrayTest()
    {
        bool[] messages = {true, false};
        Utils.TestTypeReaderWriter(new BoolArrayReadWriter(), messages);
    }

    [Test]
    public void BoolNullArrayTest()
    {
        bool[] messages = null;
        Utils.TestTypeReaderWriter(new BoolArrayReadWriter(), messages);
    }
}