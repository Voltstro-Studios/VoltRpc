using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class FloatTypeTest
    {
        [Test]
        public void FloatTest()
        {
            const float test = 80.93f;
            Utils.TestTypeReaderWriter(new FloatReadWriter(), test);
        }

        [Test]
        public void FloatArrayTest()
        {
            float[] messages = new [] {80.93f, 4.289f};
            Utils.TestTypeReaderWriter(new FloatArrayReadWriter(), messages);
        }
        
        [Test]
        public void FloatNullArrayTest()
        {
            float[] messages = null;
            Utils.TestTypeReaderWriter(new FloatArrayReadWriter(), messages);
        }
    }
}