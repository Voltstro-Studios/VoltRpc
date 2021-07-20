using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class ShortTypeTest
    {
        [Test]
        public void ShortTest()
        {
            const short test = short.MaxValue;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ShortReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            short value = (short)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void ShortArrayTest()
        {
            short[] messages = new short[] {sbyte.MaxValue, 123};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ShortArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            short[] values = (short[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}