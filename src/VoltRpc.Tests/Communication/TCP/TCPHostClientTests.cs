using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;
using VoltRpc.Tests.TestObjects.Interfaces;
using VoltRpc.Tests.TestObjects.Objects;

namespace VoltRpc.Tests.Communication.TCP;

/// <summary>
///     Tests for TCP host and client working together
/// </summary>
public class TCPHostClientTests
{
    private readonly IPEndPoint ipEndPoint = new(IPAddress.Loopback, 7777);
    
    /// <summary>
    ///     Basic connection test
    /// </summary>
    [Test]
    public async Task ConnectionTest()
    {
        //Host setup
        using TCPHost host = new(ipEndPoint);
        await StartHost(host);
        
        //Client setup
        using TCPClient client = new(ipEndPoint);
        client.Connect();
        Assert.AreEqual(true, client.IsConnected);
    }

    /// <summary>
    ///     Basic connection test with services
    /// </summary>
    [Test]
    public async Task ConnectionServicesTest()
    {
        //Host setup
        using TCPHost host = new(ipEndPoint);
        host.AddService<IBasicInterface>(new BasicObject());
        await StartHost(host);

        using TCPClient client = new(ipEndPoint);
        client.AddService<IBasicInterface>();
        client.Connect();
        Assert.AreEqual(true, client.IsConnected);
    }
    
    /// <summary>
    ///     Basic connection test with services miss-match on the client
    /// </summary>
    [Test]
    public async Task ConnectionServicesMissMatchClientTest()
    {
        using TCPHost host = new(ipEndPoint);
        host.AddService<IBasicInterface>(new BasicObject());
        await StartHost(host);

        using TCPClient client = new(ipEndPoint);
        Assert.Throws<SyncServiceMissMatchException>(() => client.Connect());
        Assert.AreEqual(false, client.IsConnected);
    }
    
    /// <summary>
    ///     Basic connection test with services miss-match on the host
    /// </summary>
    [Test]
    public async Task ConnectionServicesMissMatchHostTest()
    {
        using TCPHost host = new(ipEndPoint);
        await StartHost(host);

        using TCPClient client = new(ipEndPoint);
        client.AddService<IBasicInterface>();
        Assert.Throws<SyncServiceMissMatchException>(() => client.Connect());
        Assert.AreEqual(false, client.IsConnected);
    }
    
    /// <summary>
    ///     Basic connection test with timeout
    /// </summary>
    [Test]
    public void ConnectionTimeoutTest()
    {
        using TCPClient client = new(ipEndPoint);
        Assert.Throws<ConnectionFailedException>(() => client.Connect());
        Assert.AreEqual(false, client.IsConnected);
    }

    /// <summary>
    ///     Basic method invoke test
    /// </summary>
    [Test]
    public async Task MethodInvokeVoid()
    {
        using TCPHost host = new(ipEndPoint);
        host.AddService<IBasicInterface>(new BasicObject());
        await StartHost(host);
        
        using TCPClient client = new(ipEndPoint);
        client.AddService<IBasicInterface>();
        client.Connect();
        Assert.AreEqual(true, client.IsConnected);

        client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic");
    }
    
    /// <summary>
    ///     Basic method invoke test with exception
    /// </summary>
    [Test]
    public async Task MethodInvokeVoidException()
    {
        using TCPHost host = new(ipEndPoint);
        host.AddService<IBasicInterface>(new BasicExceptionObject());
        await StartHost(host);
        
        using TCPClient client = new(ipEndPoint);
        client.AddService<IBasicInterface>();
        client.Connect();
        Assert.AreEqual(true, client.IsConnected);

        Assert.Throws<MethodInvokeFailedException>(() =>
            client.InvokeMethod("VoltRpc.Tests.TestObjects.Interfaces.IBasicInterface.Basic"));
    }
    
    private static async Task StartHost(Host host)
    {
        _ = host.StartListeningAsync();
        while (!host.IsRunning)
        {
            await Task.Delay(10);
        }
    }
}