using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class BoolTypeTest
    {
        [Test]
        public void BoolTest()
        {
            const bool test = true;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new BoolReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            bool value = (bool)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void BoolArrayTest()
        {
            bool[] messages = new[] {true, false};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new BoolArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            bool[] values = (bool[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}