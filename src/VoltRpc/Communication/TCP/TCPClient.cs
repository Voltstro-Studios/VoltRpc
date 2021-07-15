using System.Net;
using System.Net.Sockets;

namespace VoltRpc.Communication.TCP
{
    /// <summary>
    ///     A <see cref="Client"/> that uses TCP to communicate
    /// </summary>
    public sealed class TCPClient : Client
    {
        private readonly TcpClient client;
        private readonly IPEndPoint endPoint;
        
        /// <summary>
        ///     Creates a new <see cref="TCPClient"/> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to connect to</param>
        public TCPClient(IPEndPoint endPoint)
        {
            client = new TcpClient();
            this.endPoint = endPoint;
        }

        /// <inheritdoc/>
        public override void Connect()
        {
            client.Connect(endPoint);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
            client.Dispose();
        }
    }
}