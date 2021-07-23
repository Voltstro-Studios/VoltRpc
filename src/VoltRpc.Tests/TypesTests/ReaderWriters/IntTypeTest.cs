using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class IntTypeTest
    {
        [Test]
        public void IntTest()
        {
            const int test = 73;
            Utils.TestTypeReaderWriter(new IntReadWriter(), test);
        }

        [Test]
        public void IntArrayTest()
        {
            int[] messages = new [] {73, 23};
            Utils.TestTypeReaderWriter(new IntArrayReadWriter(), messages);
        }
        
        [Test]
        public void IntNullArrayTest()
        {
            int[] messages = null;
            Utils.TestTypeReaderWriter(new IntArrayReadWriter(), messages);
        }
    }
}