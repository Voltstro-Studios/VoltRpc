using System;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Tests.TestObjects.Interfaces;
using VoltRpc.Tests.TestObjects.Objects;

namespace VoltRpc.Tests.Communication;

public abstract class CommunicationTests
{
    protected abstract void CreateClientAndHost(out Client client, out Host host);
    
    [Test]
    public void ConnectionFailTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        Assert.Throws<ConnectionFailed>(() => client.Connect());
        
        client.Dispose();
        Assert.That(client.HasDisposed);
    }
    
    [Test]
    public void ConnectionSuccessTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }

    [Test]
    public void MissingMethodClientTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListening();
        
        client.Connect();
        Assert.Throws<MissingMethodException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"), "That method does not exist on the client!");
        
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
    
    [Test]
    public void MissingMethodHostTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.StartListening();
        
        client.Connect();
        Assert.Throws<MissingMethodException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"), "That method does not exist on the host!");
        
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
    
    [Test]
    public void BasicVoidTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicObject());
        host.StartListening();
        
        client.Connect();
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic");
        Assert.IsNull(returnedObject);
        
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
    
    [Test]
    public void BasicVoidExceptionTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListening();
        
        client.Connect();
        Assert.Throws<MethodInvokeFailedException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"));
        
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
    
    [Test]
    public void BasicReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IReturnInterface>();
        
        host.AddService<IReturnInterface>(new ReturnObject());
        host.StartListening();
        
        client.Connect();
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IReturnInterface.ReturnBasic");
        Assert.IsNotNull(returnedObject);
        Assert.IsInstanceOf<object[]>(returnedObject);
        int value = (int)((object[]) returnedObject)[0];
        Assert.AreEqual(128, value);
        
        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
    
    [Test]
    public void BasicParameterTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IParameterBasicInterface>();
        
        host.AddService<IParameterBasicInterface>(new ParameterObject());
        host.StartListening();
        
        client.Connect();
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IParameterBasicInterface.BasicParam", 128);
        Assert.IsNull(returnedObject);

        Assert.That(client.IsConnected);
        
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
}