using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class UIntTypeTest
    {
        [Test]
        public void UIntTest()
        {
            const uint test = 73;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new UIntReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            uint value = (uint)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void UIntArrayTest()
        {
            uint[] messages = new uint[] {73, 23};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new UIntArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            uint[] values = (uint[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}