using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace VoltRpc.Communication.TCP
{
    /// <summary>
    ///     A <see cref="Host"/> that uses TCP to communicate
    /// </summary>
    public sealed class TCPHost : Host
    {
        private readonly TcpListener listener;

        private bool isRunning;
        
        /// <summary>
        ///     Creates a new <see cref="TCPHost"/> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to listen on.</param>
        public TCPHost(IPEndPoint endPoint)
        {
            listener = new TcpListener(endPoint);
        }
        
        /// <inheritdoc/>
        public override async Task StartListening()
        {
            isRunning = true;
            listener.Start(8192);

            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Accepted client...");
            }
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            isRunning = false;
            base.Dispose();
            listener.Stop();
        }
    }
}