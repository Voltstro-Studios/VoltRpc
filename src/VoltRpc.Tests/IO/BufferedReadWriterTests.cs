using System;
using NUnit.Framework;

namespace VoltRpc.Tests.IO;

public class BufferedReadWriterTests
{
    [Test]
    public void ByteTest()
    {
        const byte byteValue = 2;

        using DualBuffers buffers = new();

        buffers.BufferedWriter.WriteByte(byteValue);
        buffers.BufferedWriter.Flush();

        byte value = buffers.BufferedReader.ReadByte();
        Assert.AreEqual(byteValue, value);
    }

    [Test]
    public void ByteMultipleTest()
    {
        const byte byteTest1 = 2;
        const byte byteTest2 = 18;
        const byte byteTest3 = 14;

        using DualBuffers buffers = new();

        //Value 1
        buffers.BufferedWriter.WriteByte(byteTest1);
        buffers.BufferedWriter.Flush();

        byte value1 = buffers.BufferedReader.ReadByte();
        Assert.AreEqual(byteTest1, value1);

        //Value 2
        buffers.BufferedWriter.WriteByte(byteTest2);
        buffers.BufferedWriter.Flush();

        byte value2 = buffers.BufferedReader.ReadByte();
        Assert.AreEqual(byteTest2, value2);

        //Value 3
        buffers.BufferedWriter.WriteByte(byteTest3);
        buffers.BufferedWriter.Flush();

        byte value3 = buffers.BufferedReader.ReadByte();
        Assert.AreEqual(byteTest3, value3);
    }

    [Test]
    public void StringTest()
    {
        const string message = "Hello World!";

        using DualBuffers buffers = new();

        buffers.BufferedWriter.WriteString(message);
        buffers.BufferedWriter.Flush();

        string value = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message, value);
    }

    [Test]
    public void StringMultipleTest()
    {
        const string message = "Hello World!";
        const string message2 = "Fuck you Rowan!";

        using DualBuffers buffers = new();

        //Message 1
        buffers.BufferedWriter.WriteString(message);
        buffers.BufferedWriter.Flush();

        string value = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message, value);

        //Message 2
        buffers.BufferedWriter.WriteString(message2);
        buffers.BufferedWriter.Flush();

        string value2 = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message2, value2);
    }

    [Test]
    public void StringMultipleAtOnceTest()
    {
        const string message = "Hello World!";
        const string message2 = "Fuck you Rowan!";

        using DualBuffers buffers = new();

        buffers.BufferedWriter.WriteString(message);
        buffers.BufferedWriter.WriteString(message2);
        buffers.BufferedWriter.Flush();

        string value = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message, value);

        string value2 = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message2, value2);
    }

    [Test]
    public void StringWithByteTest()
    {
        const string message = "Hello World!";
        const byte byteTest = 2;

        using DualBuffers buffers = new();

        buffers.BufferedWriter.WriteString(message);
        buffers.BufferedWriter.WriteByte(byteTest);
        buffers.BufferedWriter.Flush();

        string value = buffers.BufferedReader.ReadString();
        Assert.AreEqual(message, value);

        byte byteValue = buffers.BufferedReader.ReadByte();
        Assert.AreEqual(byteTest, byteValue);
    }

    [Test]
    public void ArraySegmentTest()
    {
        byte[] baseArray = {1, 3, 4, 6, 7};
        int arraySize = baseArray.Length;
        
        using DualBuffers buffers = new();
        
        buffers.BufferedWriter.WriteBytes(baseArray, 0, arraySize);
        buffers.BufferedWriter.Flush();

        ArraySegment<byte> readSegmentArray = buffers.BufferedReader.ReadBytesSegment(arraySize);
        Assert.AreEqual(arraySize, readSegmentArray.Count);

        for (int i = 0; i < arraySize; i++)
            Assert.AreEqual(baseArray[i], readSegmentArray[i]);
    }
}