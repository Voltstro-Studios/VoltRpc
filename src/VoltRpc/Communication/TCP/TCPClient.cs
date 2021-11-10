using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace VoltRpc.Communication.TCP;

/// <summary>
///     A <see cref="Client" /> that uses TCP to communicate
/// </summary>
public sealed class TCPClient : Client
{
    /// <summary>
    ///     Default receive timeout time
    /// </summary>
    public const int DefaultReceiveTimeout = 600000;

    /// <summary>
    ///     Default send timeout time
    /// </summary>
    public const int DefaultSendTimeout = 600000;

    /// <summary>
    ///     Default connection timeout time
    /// </summary>
    public const int DefaultConnectionTimeout = 7000;

    private readonly TcpClient client;
    private readonly int connectionTimeout;
    private readonly IPEndPoint endPoint;

    private Stream clientStream;

    /// <summary>
    ///     Creates a new <see cref="TCPClient" /> instance
    /// </summary>
    /// <param name="endPoint">The <see cref="IPEndPoint" /> to connect to</param>
    /// <param name="bufferSize">The size of the buffers</param>
    /// <param name="connectionTimeout">The timeout time for connection</param>
    /// <param name="receiveTimeout">The receive timeout</param>
    /// <param name="sendTimeout">The send timeout</param>
    public TCPClient(IPEndPoint endPoint, int bufferSize = DefaultBufferSize,
        int connectionTimeout = DefaultConnectionTimeout,
        int receiveTimeout = DefaultReceiveTimeout,
        int sendTimeout = DefaultSendTimeout)
        : base(bufferSize)
    {
        client = new TcpClient
        {
            ReceiveTimeout = receiveTimeout,
            SendTimeout = sendTimeout
        };
        this.endPoint = endPoint;
        this.connectionTimeout = connectionTimeout;
    }

    /// <summary>
    ///     Creates a new <see cref="TCPClient" /> instance
    /// </summary>
    /// <param name="endPoint">The <see cref="IPEndPoint" /> to connect to</param>
    /// <param name="receiveTimeout">The receive timeout</param>
    /// <param name="sendTimeout">The send timeout</param>
    public TCPClient(IPEndPoint endPoint, int receiveTimeout, int sendTimeout)
        : this(endPoint, DefaultBufferSize, DefaultConnectionTimeout, receiveTimeout, sendTimeout)
    {
    }

    /// <summary>
    ///     Creates a new <see cref="TCPClient" /> instance
    /// </summary>
    /// <param name="endPoint">The <see cref="IPEndPoint" /> to connect to</param>
    /// <param name="connectionTimeout">The timeout time for connection</param>
    public TCPClient(IPEndPoint endPoint, int connectionTimeout)
        : this(endPoint, DefaultBufferSize, connectionTimeout)
    {
    }

    /// <summary>
    ///     Creates a new <see cref="TCPClient" /> instance
    /// </summary>
    /// <param name="endPoint">The <see cref="IPEndPoint" /> to connect to</param>
    public TCPClient(IPEndPoint endPoint)
        : this(endPoint, DefaultBufferSize)
    {
    }

    /// <inheritdoc />
    /// <exception cref="TimeoutException">Thrown if a connection timeout occurs</exception>
    /// <exception cref="ConnectionFailed">Thrown if an unknown error occurs while connecting.</exception>
    public override void Connect()
    {
        try
        {
            if (!client.ConnectAsync(endPoint.Address, endPoint.Port).Wait(connectionTimeout))
                throw new TimeoutException("The TCP client failed to connect in time.");
        }
        catch (Exception ex)
        {
            throw new ConnectionFailed("The TCP client failed to connect!", ex);
        }

        clientStream = client.GetStream();
        Initialize(clientStream, clientStream);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        base.Dispose();
        clientStream.Dispose();
        client.Dispose();
    }
}