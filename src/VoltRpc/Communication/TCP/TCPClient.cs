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
        private readonly int timeout;

        /// <summary>
        ///     Creates a new <see cref="TCPClient"/> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to connect to</param>
        /// <param name="timeout">The timeout for connection</param>
        public TCPClient(IPEndPoint endPoint, int timeout = 2000)
        {
            client = new TcpClient();
            this.endPoint = endPoint;
            this.timeout = timeout;
        }

        /// <inheritdoc/>
        /// <exception cref="TimeoutException"></exception>
        public override void Connect()
        {
            client.BeginConnect(endPoint.Address, endPoint.Port, result =>
            {
                IsConnectedInternal = true;
            }, client);
            
            while (!IsConnectedInternal)
            {
                if (SpinWait.SpinUntil(() => IsConnectedInternal, timeout))
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