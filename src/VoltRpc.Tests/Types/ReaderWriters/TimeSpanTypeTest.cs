using System;
using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class TimeSpanTypeTest
{
    [Test]
    public void TimeSpanTest()
    {
        TimeSpan timeSpan = DateTime.Now.TimeOfDay;
        Utils.TestTypeReaderWriter(new TimeSpanReadWriter(), timeSpan);
    }
}