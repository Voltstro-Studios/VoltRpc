using System;
using System.Numerics;
using NUnit.Framework;
using VoltRpc.Extension.Vectors.Types;
using VoltRpc.Tests.IO;
using VoltRpc.Types;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types;

public class ReaderWritersTests
{
    [TestCaseSource(nameof(typesToTest))]
    public void TypeReaderWriterTest<T>(TypeReadWriter<T> typeReadWriter, T value)
    {
        using DualBuffers buffers = new();
        typeReadWriter.Write(buffers.BufferedWriter, value);
        buffers.BufferedWriter.Flush();

        T result = typeReadWriter.Read(buffers.BufferedReader);
        Assert.AreEqual(value, result);
    }

    private static object[] typesToTest =
    {
        //Base
        new object[] { new BoolReadWriter(), true },
        new object[] { new ByteReadWriter(), (byte)128 },
        new object[] { new CharReadWriter(), 'g'},
        new object[] { new DateTimeReadWriter(), DateTime.Now },
        new object[] { new DecimalReadWriter(), decimal.MaxValue },
        new object[] { new DoubleReadWriter(), double.MaxValue },
        new object[] { new FloatReadWriter(), float.MaxValue },
        new object[] { new IntReadWriter(), int.MaxValue },
        new object[] { new LongReadWriter(), long.MaxValue },
        new object[] { new SByteReadWriter(), sbyte.MaxValue },
        new object[] { new ShortReadWriter(), short.MaxValue },
        new object[] { new StringReadWriter(), "Rowan SUXS" },
        new object[] { new TimeSpanReadWriter(), DateTime.Now.TimeOfDay },
        new object[] { new UIntReadWriter(), uint.MaxValue },
        new object[] { new ULongReadWriter(), ulong.MaxValue },
        new object[] { new UriReadWriter(), new Uri("https://voltstro.dev") },
        new object[] { new UShortReadWriter(), ushort.MaxValue },
        
        //Extensions
        new object[] { new Matrix3X2TypeReadWriter(), new Matrix3x2(14, 18, 22, 17, 18, 124) },
        new object[] { new Matrix4X4TypeReadWriter(), new Matrix4x4(14, 18, 22, 17, 18, 124, 8, 2, 454, 33, 1, 4389, 4, 4, 4, 7) },
        new object[] { new PlaneTypeReadWriter(), new Plane(128, 62, 45, 8) },
        new object[] { new QuaternionTypeReadWriter(), Quaternion.CreateFromAxisAngle(Vector3.UnitX, 32f) },
        new object[] { new Vector2TypeReadWriter(), Vector2.One },
        new object[] { new Vector3TypeReadWriter(), Vector3.One },
        new object[] { new Vector4TypeReadWriter(), Vector4.One }
    };
}