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

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new FloatReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            float value = (float)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void FloatArrayTest()
        {
            float[] messages = new [] {80.93f, 4.289f};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new FloatArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            float[] values = (float[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}