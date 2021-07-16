using System;
using System.IO;
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

            //TODO: Support multiple clients at once
            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Accepted client...");

                //Start processing requests from the client
                BufferedStream stream = new BufferedStream(client.GetStream(), 8192);
                ProcessRequest(stream, stream);
                
                //Connection was closed
                stream.Dispose();
                client.Dispose();
                Console.WriteLine("Client disconnected.");
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