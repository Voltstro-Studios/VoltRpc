using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class ByteTypeTest
    {
        [Test]
        public void ByteTest()
        {
            const byte test = 25;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ByteReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            byte value = (byte)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void ByteArrayTest()
        {
            byte[] messages = new byte[] {13, 40};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ByteArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            byte[] values = (byte[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}