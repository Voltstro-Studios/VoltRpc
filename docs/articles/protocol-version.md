# Protocol Version

In VoltRpc, you can set a "protocol version". The value that you want to use for your protocol version has to be set on both the client and host. The value will then be checked when a client connects to a host, and if they are the same, then the connection will be approved, if not, then the connection will be refused.

The protocol version value can be any <xref:System.Type> that has a <xref:VoltRpc.Types.TypeReadWriter`1> for it. This means custom <xref:System.Type>s can be used that has a custom <xref:VoltRpc.Types.TypeReadWriter`1> for it (see [types for more info](types.md).)

Internally, VoltRpc uses the <xref:System.Object.Equals(System.Object)> for comparison of values (VoltRpc will already check that the type sent by the client matches the host). Please remember this when using custom types.

## Configuring

To set what protocol version you want to use, you will need to use <xref:VoltRpc.Communication.Host.SetProtocolVersion(System.Object)> on the host, and <xref:VoltRpc.Communication.Client.SetProtocolVersion(System.Object)> on the client.

Ideally, use a <xref:System.Type> that has a small data size (such a <xref:System.Byte>).

When you are ready, you can change the protocol version value used by the host to stop all un-wanted connections from clients that have different protocol versions.