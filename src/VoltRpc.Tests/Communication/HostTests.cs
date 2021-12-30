using System;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Services;
using VoltRpc.Tests.Communication.TestObjects;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Communication;

public class HostTests
{
    [Test]
    public void ServiceNotInterfaceTest()
    {
        DualBuffers buffers = new();
        Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);
        Assert.Throws<ArgumentOutOfRangeException>(() => host.AddService(new TestClass()));
        buffers.Dispose();
    }

    [Test]
    public void ServiceInterfaceAddTest()
    {
        DualBuffers buffers = new();
        Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);

        TestClass testClass = new();
        string interfaceName = typeof(IInterface).FullName;

        host.AddService<IInterface>(testClass);

        HostService service = host.Services[0];
        Assert.AreEqual(service.InterfaceName, interfaceName);
        Assert.AreEqual(testClass, service.InterfaceObject);
        
        buffers.Dispose();
    }

    [Test]
    public void ServiceInterfaceAlreadyAddedTest()
    {
        DualBuffers buffers = new();
        Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);

        TestClass testClass = new();
        string interfaceName = typeof(IInterface).FullName;

        host.AddService<IInterface>(testClass);

        HostService service = host.Services[0];
        Assert.AreEqual(service.InterfaceName, interfaceName);
        Assert.AreEqual(testClass, service.InterfaceObject);
        
        Assert.Throws<ArgumentException>(() => host.AddService<IInterface>(testClass));
        buffers.Dispose();
    }

    [Test]
    public void DisposeBeforeListeningTest()
    {
        DualBuffers buffers = new();
        Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);
        
        host.Dispose();
        Assert.That(!host.IsRunning);
        Assert.That(host.HasDisposed);
    }

    [Test]
    public void DisposeAfterListeningTest()
    {
        DualBuffers buffers = new();
        Host host = new MemoryStreamHost(buffers.BufferedReader, buffers.BufferedWriter);
        host.StartListening();
        
        host.Dispose();
        Assert.That(!host.IsRunning);
        Assert.That(host.HasDisposed);
    }
}