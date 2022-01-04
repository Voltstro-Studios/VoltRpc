using System;
using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class DateTimeTypeTest
{
    [Test]
    public void DateTimeTest()
    {
        DateTime dateTime = DateTime.Now;
        Utils.TestTypeReaderWriter(new DateTimeReadWriter(), dateTime);
    }
}