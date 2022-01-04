using System;
using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.Types.ReaderWriters;

public class UriTypeTest
{
    [Test]
    public void UriTest()
    {
        Uri uri = new Uri("https://voltstro.dev");
        Utils.TestTypeReaderWriter(new UriReadWriter(), uri);
    }
    
    [Test]
    public void UriNullTest()
    {
        Utils.TestTypeReaderWriter(new UriReadWriter(), null);
    }
}