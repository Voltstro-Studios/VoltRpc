using System;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Tests.Communication.TestObjects;
using VoltRpc.Tests.IO;

namespace VoltRpc.Tests.Communication;

public class ClientTests
{
    [Test]
    public void ServiceNotInterfaceTest()
    {
        DualBuffers buffers = new();
        Client client = new MemoryStreamClient(buffers.BufferedReader, buffers.BufferedWriter);
        Assert.Throws<ArgumentOutOfRangeException>(() => client.AddService<TestClass>());
        buffers.Dispose();
    }

    [Test]
    public void ServiceInterfaceAddTest()
    {
        DualBuffers buffers = new();
        Client client = new MemoryStreamClient(buffers.BufferedReader, buffers.BufferedWriter);
        client.AddService<IInterface>();

        string interfaceName = typeof(IInterface).FullName;
        Assert.That(client.Services.ContainsKey(interfaceName));

        buffers.Dispose();
    }

    [Test]
    public void ServiceInterfaceAlreadyAddedTest()
    {
        DualBuffers buffers = new();
        Client client = new MemoryStreamClient(buffers.BufferedReader, buffers.BufferedWriter);
        client.AddService<IInterface>();

        string interfaceName = typeof(IInterface).FullName;
        Assert.That(client.Services.ContainsKey(interfaceName));

        Assert.Throws<ArgumentOutOfRangeException>(() => client.AddService<IInterface>());

        buffers.Dispose();
    }

    [Test]
    public void DisposeBeforeConnectingTest()
    {
        DualBuffers buffers = new();
        Client client = new MemoryStreamClient(buffers.BufferedReader, buffers.BufferedWriter);

        client.Dispose();
        Assert.That(!client.IsConnected);
        Assert.That(client.HasDisposed);
    }
}