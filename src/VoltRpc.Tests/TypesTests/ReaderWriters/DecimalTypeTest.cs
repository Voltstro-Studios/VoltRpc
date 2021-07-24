using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class DecimalTypeTest
    {
        [Test]
        public void DecimalTest()
        {
            const decimal test = 3;
            Utils.TestTypeReaderWriter(new DecimalReadWriter(), test);
        }

        [Test]
        public void DecimalArrayTest()
        {
            decimal[] messages = new decimal[] {3, 8};
            Utils.TestTypeReaderWriter(new DecimalArrayReadWriter(), messages);
        }
        
        [Test]
        public void DecimalNullArrayTest()
        {
            decimal[] messages = null;
            Utils.TestTypeReaderWriter(new DecimalArrayReadWriter(), messages);
        }
    }
}