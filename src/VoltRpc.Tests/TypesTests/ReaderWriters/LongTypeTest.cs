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

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new LongReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            long value = (long)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void LongArrayTest()
        {
            long[] messages = new [] {long.MaxValue, 80};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new LongArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            long[] values = (long[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}