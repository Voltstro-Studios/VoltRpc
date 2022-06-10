using System;
using System.Threading;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Tests.TestObjects.Interfaces;
using VoltRpc.Tests.TestObjects.Objects;

namespace VoltRpc.Tests.Communication;

/// <summary>
///     Base layer for communication tests
/// </summary>
public abstract class CommunicationTests
{
    protected abstract void CreateClientAndHost(out Client client, out Host host);

    #region Connection

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
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        DisposeClientAndHost(client, host);
    }

    [Test]
    public void ConnectionFailVersionMissMatch()
    {
        CreateClientAndHost(out Client client, out Host host);
        host.version = new Versioning.VersionInfo
        {
            Major = 0,
            Minor = 1,
            Patch = 2
        };
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);

        Assert.Throws<VersionMissMatchException>(() => client.Connect());
        
        DisposeClientAndHost(client, host);
    }

    #endregion

    [Test]
    public void MethodMissingClientTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
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
        
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        Assert.Throws<MissingMethodException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"), "That method does not exist on the host!");

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic");
        Assert.IsNull(returnedObject);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidExceptionTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IBasicInterface>();
        
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        Assert.Throws<MethodInvokeFailedException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"));
        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodReturnTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IReturnInterface>();
        
        host.AddService<IReturnInterface>(new ReturnObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IReturnInterface.ReturnBasic");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);
        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IParameterBasicInterface>();
        
        host.AddService<IParameterBasicInterface>(new ParameterObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IParameterBasicInterface.BasicParam", 128);
        Assert.IsNull(returnedObject);

        
        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterRefTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefBasicInterface>();
        
        host.AddService<IRefBasicInterface>(new RefObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefBasicInterface.RefBasic", 75);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodReturnParameterRefTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefReturnInterface>();
        
        host.AddService<IRefReturnInterface>(new RefReturnObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefReturnInterface.RefReturn", 75);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        
        Assert.AreEqual(25, returnedObjects[0]);
        Assert.AreEqual(128, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterOutTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IOutInterface>();
        
        host.AddService<IOutInterface>(new OutObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IOutInterface.OutBasic");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 1);
        Assert.AreEqual(128, returnedObjects[0]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodReturnParameterOutTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IOutReturnInterface>();
        
        host.AddService<IOutReturnInterface>(new OutReturnObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IOutReturnInterface.OutReturn");
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        Assert.AreEqual(75, returnedObjects[0]);
        Assert.AreEqual(128, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterOutRefTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefOutInterface>();
        
        host.AddService<IRefOutInterface>(new RefOutObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefOutInterface.RefOutBasic", 128);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 2);
        Assert.AreEqual(75, returnedObjects[0]);
        Assert.AreEqual(25, returnedObjects[1]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodReturnParameterOutRefTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IRefOutReturnInterface>();
        
        host.AddService<IRefOutReturnInterface>(new RefOutReturnObject());
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        object returnedObject = client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IRefOutReturnInterface.RefOutReturn", 128);
        object[] returnedObjects = GetReturnedObjects(returnedObject, 3);
        Assert.AreEqual(16, returnedObjects[0]);
        Assert.AreEqual(75, returnedObjects[1]);
        Assert.AreEqual(25, returnedObjects[2]);

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterArrayTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IArrayBasicInterface>();
        
        host.AddService<IArrayBasicInterface>(new ArrayBasicInterface(false));
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IArrayBasicInterface.Array", new byte[]{1, 3 ,4, 8});

        DisposeClientAndHost(client, host);
    }
    
    [Test]
    public void MethodVoidParameterArrayNullTest()
    {
        CreateClientAndHost(out Client client, out Host host);
        client.AddService<IArrayBasicInterface>();
        
        host.AddService<IArrayBasicInterface>(new ArrayBasicInterface(true));
        host.StartListeningAsync().ConfigureAwait(false);
        Thread.Sleep(100);
        
        client.Connect();
        Assert.That(client.IsConnected);
        
        client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IArrayBasicInterface.Array", new object[] {null});

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