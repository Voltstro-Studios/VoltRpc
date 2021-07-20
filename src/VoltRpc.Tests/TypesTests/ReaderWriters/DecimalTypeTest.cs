using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class DecimalTypeTest
    {
        [Test]
        public void DecimalTest()
        {
            const decimal test = 3;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new DecimalReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            decimal value = (decimal)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void DecimalArrayTest()
        {
            decimal[] messages = new decimal[] {3, 8};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new DecimalArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            decimal[] values = (decimal[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}