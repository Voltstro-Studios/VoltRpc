using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class DoubleTypeTest
    {
        [Test]
        public void DoubleTest()
        {
            const double test = 123.48;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new DoubleReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            double value = (double)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void DoubleArrayTest()
        {
            double[] messages = new [] {128.32, 700.4};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new DoubleArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            double[] values = (double[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}