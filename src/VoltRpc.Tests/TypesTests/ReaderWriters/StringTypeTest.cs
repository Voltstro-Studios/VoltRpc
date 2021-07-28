using NUnit.Framework;
using VoltRpc.Types.TypeReaderWriters;

namespace VoltRpc.Tests.TypesTests.ReaderWriters
{
    public class StringTypeTest
    {
        [Test]
        public void StringTest()
        {
            const string test = "Hello World!";
            Utils.TestTypeReaderWriter(new StringReadWriter(), test);
        }

        [Test]
        public void StringNullTest()
        {
            const string test = null;
            Utils.TestTypeReaderWriter(new StringReadWriter(), test);
        }

        [Test]
        public void StringArrayTest()
        {
            string[] messages = {"Hello World!", "Rowan Suxs"};
            Utils.TestTypeReaderWriter(new StringArrayReadWriter(), messages);
        }

        [Test]
        public void StringNullArrayTest()
        {
            string[] messages = null;
            Utils.TestTypeReaderWriter(new StringArrayReadWriter(), messages);
        }
    }
}