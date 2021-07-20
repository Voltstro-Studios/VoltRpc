using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class UShortTypeTest
    {
        [Test]
        public void UShortTest()
        {
            const ushort test = ushort.MaxValue;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new UShortReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            ushort value = (ushort)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void UShortArrayTest()
        {
            ushort[] messages = new ushort[] {ushort.MaxValue, 800};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new UShortArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            ushort[] values = (ushort[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}