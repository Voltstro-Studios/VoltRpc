using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class ULongTypeTest
    {
        [Test]
        public void ULongTest()
        {
            const ulong test = ulong.MaxValue;

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ULongReadWriter();
            readWriter.Write(buffers.BufferedWriter, test);
            buffers.BufferedWriter.Flush();

            ulong value = (ulong)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(test, value);
        }

        [Test]
        public void ULongArrayTest()
        {
            ulong[] messages = new ulong[] {ulong.MaxValue, 800};

            using DualBuffers buffers = new DualBuffers();

            ITypeReadWriter readWriter = new ULongArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            ulong[] values = (ulong[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}