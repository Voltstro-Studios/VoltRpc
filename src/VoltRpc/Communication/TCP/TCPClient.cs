using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace VoltRpc.Communication.TCP
{
    /// <summary>
    ///     A <see cref="Client"/> that uses TCP to communicate
    /// </summary>
    public sealed class TCPClient : Client
    {
        private readonly TcpClient client;
        
        private readonly IPEndPoint endPoint;
        private readonly int connectionTimeout;

        /// <summary>
        ///     Creates a new <see cref="TCPClient"/> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to connect to</param>
        /// <param name="connectionTimeout">The timeout for connection</param>
        /// <param name="receiveTimeout">The receive timeout</param>
        /// <param name="sendTimeout">The send timeout</param>
        public TCPClient(IPEndPoint endPoint, int connectionTimeout = 2000, int receiveTimeout = 600000, int sendTimeout = 600000)
        {
            client = new TcpClient
            {
                ReceiveTimeout = receiveTimeout, 
                SendTimeout = sendTimeout
            };
            this.endPoint = endPoint;
            this.connectionTimeout = connectionTimeout;
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException">Thrown if a connection timeout occurs</exception>
        public override void Connect()
        {
            client.BeginConnect(endPoint.Address, endPoint.Port, result =>
            {
                IsConnectedInternal = true;
            }, client);
            
            while (!IsConnectedInternal)
            {
                if (SpinWait.SpinUntil(() => IsConnectedInternal, connectionTimeout))
                    continue;
                
                throw new TimeoutException($"Client failed to connect to {endPoint}!");
            }

            Initialize(new BufferedStream(client.GetStream(), 8192));
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
            client.Dispose();
            IsConnectedInternal = false;
        }
    }
}