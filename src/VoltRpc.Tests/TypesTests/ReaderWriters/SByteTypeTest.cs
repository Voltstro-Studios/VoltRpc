using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class SByteTypeTest
    {
        [Test]
        public void SByteTest()
        {
            const sbyte test = sbyte.MaxValue;
            Utils.TestTypeReaderWriter(new SByteReadWriter(), test);
        }

        [Test]
        public void SByteArrayTest()
        {
            sbyte[] messages = {sbyte.MaxValue, 13};
            Utils.TestTypeReaderWriter(new SByteArrayReadWriter(), messages);
        }

        [Test]
        public void SByteNullArrayTest()
        {
            sbyte[] messages = null;
            Utils.TestTypeReaderWriter(new SByteArrayReadWriter(), messages);
        }
    }
}