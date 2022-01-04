using System.Net;
using VoltRpc.Communication;
using VoltRpc.Communication.TCP;

namespace VoltRpc.Tests.Communication.TCP;

public class TCPTests : CommunicationTests
{
    protected override void CreateClientAndHost(out Client client, out Host host)
    {
        IPEndPoint ip = new(IPAddress.Loopback, 6645);
        client = new TCPClient(ip);
        host = new TCPHost(ip);
    }
}