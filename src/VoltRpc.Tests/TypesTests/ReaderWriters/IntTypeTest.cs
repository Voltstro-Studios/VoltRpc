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

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new IntReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            int value = (int)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void IntArrayTest()
        {
            int[] messages = new [] {73, 23};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new IntArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            int[] values = (int[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}