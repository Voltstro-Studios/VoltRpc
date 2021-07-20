using NUnit.Framework;
using VoltRpc.Tests.IO;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class StringTypeTest
    {
        [Test]
        public void StringTest()
        {
            const string message = "Hello World!";

            using DualBuffers buffers = new DualBuffers();

            StringReadWriter readWriter = new StringReadWriter();
            readWriter.Write(buffers.BufferedWriter, message);
            buffers.BufferedWriter.Flush();

            string value = (string)readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(message, value);
        }

        [Test]
        public void StringArrayTest()
        {
            string[] messages = new[] {"Hello World!", "Rowan Suxs"};

            using DualBuffers buffers = new DualBuffers();

            StringArrayReadWriter readWriter = new StringArrayReadWriter();
            readWriter.Write(buffers.BufferedWriter, messages);
            buffers.BufferedWriter.Flush();

            string[] values = (string[])readWriter.Read(buffers.BufferedReader);
            Assert.AreEqual(messages, values);
        }
    }
}