using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class ShortTypeTest
    {
        [Test]
        public void ShortTest()
        {
            const short test = short.MaxValue;
            Utils.TestTypeReaderWriter(new ShortReadWriter(), test);
        }

        [Test]
        public void ShortArrayTest()
        {
            short[] messages = new short[] {sbyte.MaxValue, 123};
            Utils.TestTypeReaderWriter(new ShortArrayReadWriter(), messages);
        }
        
        [Test]
        public void ShortNullArrayTest()
        {
            short[] messages = null;
            Utils.TestTypeReaderWriter(new ShortArrayReadWriter(), messages);
        }
    }
}