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
        CreateClientAndHost(out Client client, out Host _);
        Assert.Throws<ConnectionFailedException>(() => client.Connect());
        
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
        
        DisposeClientAndHost(client, host);
    }

    [Test]
    public void MethodMissingClientTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        Assert.Throws<MissingMethodException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"), "That method does not exist on the client!");

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodMissingHostTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        Assert.Throws<MissingMethodException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"), "That method does not exist on the host!");

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void BasicVoidTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic");
        Assert.IsNull(returnedObject);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void BasicVoidExceptionTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        Assert.Throws<MethodInvokeFailedException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"));
        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void BasicReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IReturnInterface>();
        
        host.AddService<IReturnInterface>(new ReturnObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IReturnInterface.ReturnBasic");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);
        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IParameterBasicInterface>();
        
        host.AddService<IParameterBasicInterface>(new ParameterObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IParameterBasicInterface.BasicParam", 128);
        Assert.IsNull(returnedObject);

        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterRefTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefBasicInterface>();
        
        host.AddService<IRefBasicInterface>(new RefObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefBasicInterface.RefBasic", 75);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterRefReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefReturnInterface>();
        
        host.AddService<IRefReturnInterface>(new RefReturnObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefReturnInterface.RefReturn", 75);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        
        Assert.AreEqual(25, returnedObjects[0]);
        Assert.AreEqual(128, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterOutTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IOutInterface>();
        
        host.AddService<IOutInterface>(new OutObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IOutInterface.OutBasic");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterOutReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IOutReturnInterface>();
        
        host.AddService<IOutReturnInterface>(new OutReturnObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IOutReturnInterface.OutReturn");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        Assert.AreEqual(75, returnedObjects[0]);
        Assert.AreEqual(128, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterRefOutTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefOutInterface>();
        
        host.AddService<IRefOutInterface>(new RefOutObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefOutInterface.RefOutBasic", 128);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        Assert.AreEqual(75, returnedObjects[0]);
        Assert.AreEqual(25, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void ParameterRefOutReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefOutReturnInterface>();
        
        host.AddService<IRefOutReturnInterface>(new RefOutReturnObject());
        host.StartListening();
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefOutReturnInterface.RefOutReturn", 128);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 3);
        Assert.AreEqual(16, returnedObjects[0]);
        Assert.AreEqual(75, returnedObjects[1]);
        Assert.AreEqual(25, returnedObjects[2]);

        DisposeClientAndHost(client, host);
    }

    private static object[] GetReturnedObjects(object returnedObject, int expectedLenght)
    {
        Assert.IsNotNull(returnedObject);
        Assert.IsInstanceOf<object[]>(returnedObject);
        object[] returnedObjects = (object[]) returnedObject;
        Assert.AreEqual(expectedLenght, returnedObjects.Length);

        return returnedObjects;
    }

    private static void DisposeClientAndHost(Client client, Host host)
    {
        client.Dispose();
        host.Dispose();
        
        Assert.That(client.HasDisposed);
        Assert.That(host.HasDisposed);
    }
}