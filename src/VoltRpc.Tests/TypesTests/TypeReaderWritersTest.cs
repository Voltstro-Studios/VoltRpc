using System.IO;
using NUnit.Framework;
using VoltRpc.IO;
using VoltRpc.Tests.IO;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests
{
    public class TypeReaderWritersTest
    {
        [Test]
        public void StringTypeTest()
        {
            const string message = "Hello World!";
            
            using MemoryStream ms = new MemoryStream(1000);
            using BufferedWriter writer = new MemoryStreamBufferedWriter(ms);
            using BufferedReader reader = new MemoryStreamBufferedReader(ms);

            StringReadWriter readWriter = new StringReadWriter();
            readWriter.Write(writer, message);
            writer.Flush();

            string value = (string)readWriter.Read(reader);
            Assert.AreEqual(message, value);
        }

        [Test]
        public void StringArrayTypeTest()
        {
            string[] messages = new[] {"Hello World!", "Rowan Suxs"};
            
            using MemoryStream ms = new MemoryStream(1000);
            using BufferedWriter writer = new MemoryStreamBufferedWriter(ms);
            using BufferedReader reader = new MemoryStreamBufferedReader(ms);

            StringArrayReadWriter readWriter = new StringArrayReadWriter();
            readWriter.Write(writer, messages);
            writer.Flush();

            string[] values = (string[])readWriter.Read(reader);
            Assert.AreEqual(messages, values);
        }
    }
}