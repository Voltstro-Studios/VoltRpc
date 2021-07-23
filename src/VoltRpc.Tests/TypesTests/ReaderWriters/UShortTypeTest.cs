using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class UShortTypeTest
    {
        [Test]
        public void UShortTest()
        {
            const ushort test = ushort.MaxValue;
            Utils.TestTypeReaderWriter(new UShortReadWriter(), test);
        }

        [Test]
        public void UShortArrayTest()
        {
            ushort[] messages = new ushort[] {ushort.MaxValue, 800};
            Utils.TestTypeReaderWriter(new UShortArrayReadWriter(), messages);
        }
        
        [Test]
        public void UShortNullArrayTest()
        {
            ushort[] messages = null;
            Utils.TestTypeReaderWriter(new UShortArrayReadWriter(), messages);
        }
    }
}