using System.IO;
using NUnit.Framework;
using VoltRpc.IO;

namespace VoltRpc.Tests.IOTests
{
    public class BufferedReadWriterTests
    {
        [Test]
        public void ByteTest()
        {
            const byte byteValue = 2;
            
            using MemoryStream ms = new MemoryStream(1000);
            using BufferedWriter writer = new MemoryStreamBufferedWriter(ms);
            using BufferedReader reader = new MemoryStreamBufferedReader(ms);
            
            writer.WriteByte(byteValue);
            writer.Flush();

            byte value = reader.ReadByte();
            Assert.AreEqual(byteValue, value);
        }
        
        [Test]
        public void ByteMultipleTest()
        {
            const byte byteTest1 = 2;
            const byte byteTest2 = 18;
            const byte byteTest3 = 14;
            
            using MemoryStream ms = new MemoryStream(1000);
            using BufferedWriter writer = new MemoryStreamBufferedWriter(ms);
            using BufferedReader reader = new MemoryStreamBufferedReader(ms);
            
            //Value 1
            writer.WriteByte(byteTest1);
            writer.Flush();

            byte value1 = reader.ReadByte();
            Assert.AreEqual(byteTest1, value1);
            
            //Value 2
            writer.WriteByte(byteTest2);
            writer.Flush();

            byte value2 = reader.ReadByte();
            Assert.AreEqual(byteTest2, value2);
            
            //Value 3
            writer.WriteByte(byteTest3);
            writer.Flush();

            byte value3 = reader.ReadByte();
            Assert.AreEqual(byteTest3, value3);
        }
    }
}