using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class SByteTypeTest
    {
        [Test]
        public void SByteTest()
        {
            const sbyte test = sbyte.MaxValue;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new SByteReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            sbyte value = (sbyte)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void SByteArrayTest()
        {
            sbyte[] messages = new sbyte[] {sbyte.MaxValue, 13};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new SByteArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            sbyte[] values = (sbyte[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}