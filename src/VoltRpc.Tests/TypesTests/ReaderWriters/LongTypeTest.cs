using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class LongTypeTest
    {
        [Test]
        public void LongTest()
        {
            const long test = long.MaxValue;
            Utils.TestTypeReaderWriter(new LongReadWriter(), test);
        }

        [Test]
        public void LongArrayTest()
        {
            long[] messages = new [] {long.MaxValue, 80};
            Utils.TestTypeReaderWriter(new LongArrayReadWriter(), messages);
        }
        
        [Test]
        public void LongNullArrayTest()
        {
            long[] messages = null;
            Utils.TestTypeReaderWriter(new LongArrayReadWriter(), messages);
        }
    }
}