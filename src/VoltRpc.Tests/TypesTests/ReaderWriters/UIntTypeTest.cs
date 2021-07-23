using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class UIntTypeTest
    {
        [Test]
        public void UIntTest()
        {
            const uint test = 73;
            Utils.TestTypeReaderWriter(new UIntReadWriter(), test);
        }

        [Test]
        public void UIntArrayTest()
        {
            uint[] messages = new uint[] {73, 23};
            Utils.TestTypeReaderWriter(new UIntArrayReadWriter(), messages);
        }
        
        [Test]
        public void UIntNullArrayTest()
        {
            uint[] messages = null;
            Utils.TestTypeReaderWriter(new UIntArrayReadWriter(), messages);
        }
    }
}