using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VoltRpc.Communication.TCP
{
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
            int receiveTimeout = DefaultReceiveTimeout, int sendTimeout = DefaultSendTimeout)
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
        /// <param name="connectionTimeout">The timeout time for connection</param>
        public TCPClient(IPEndPoint endPoint, int connectionTimeout = DefaultConnectionTimeout)
            : this(endPoint, DefaultBufferSize, connectionTimeout)
        {
        }

        /// <inheritdoc />
        /// <exception cref="TimeoutException">Thrown if a connection timeout occurs</exception>
        public override void Connect()
        {
            client.BeginConnect(endPoint.Address, endPoint.Port, result => { IsConnectedInternal = true; }, client);

            while (!IsConnectedInternal)
            {
                if (SpinWait.SpinUntil(() => IsConnectedInternal, connectionTimeout))
                    continue;

                throw new TimeoutException($"Client failed to connect to {endPoint}!");
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
            IsConnectedInternal = false;
        }
    }
}