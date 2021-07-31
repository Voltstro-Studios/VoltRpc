using System;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Communication
{
    public class HostTests
    {
        [Test]
        public void ServiceNotInterfaceTest()
        {
            DualBuffers buffers = new();
            Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);
            Assert.Throws<ArgumentOutOfRangeException>(() => host.AddService<TestClass>(new TestClass()));
            buffers.Dispose();
        }
        
        [Test]
        public void InterfaceAlreadyAddedTest()
        {
            DualBuffers buffers = new();
            Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);

            TestClass testClass = new();
            
            host.AddService<IInterface>(testClass);
            Assert.Throws<ArgumentException>(() => host.AddService<IInterface>(testClass));
            buffers.Dispose();
        }
        
        private interface IInterface
        {
        }

        private class TestClass : IInterface
        {
        }
    }
}