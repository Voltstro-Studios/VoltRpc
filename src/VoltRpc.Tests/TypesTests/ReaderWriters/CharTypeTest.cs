using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class CharTypeTest
    {
        [Test]
        public void CharTest()
        {
            const char test = 'g';

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new CharReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            char value = (char)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void CharArrayTest()
        {
            char[] messages = new [] {'g', 'L'};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new CharArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            char[] values = (char[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}