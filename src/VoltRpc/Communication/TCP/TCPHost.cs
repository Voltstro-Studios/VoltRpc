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

        private readonly int receiveTimeout;
        private readonly int sendTimeout;

        /// <summary>
        ///     Creates a new <see cref="TCPHost"/> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to listen on.</param>
        /// <param name="receiveTimeout"></param>
        /// <param name="sendTimeout"></param>
        public TCPHost(IPEndPoint endPoint, int receiveTimeout = 600000, int sendTimeout = 600000)
        {
            listener = new TcpListener(endPoint);
            this.receiveTimeout = receiveTimeout;
            this.sendTimeout = sendTimeout;
        }
        
        /// <inheritdoc/>
        public override async Task StartListening()
        {
            isRunning = true;
            listener.Start(8192);
            
            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                client.ReceiveTimeout = receiveTimeout;
                client.SendTimeout = sendTimeout;

                Console.WriteLine("Accepted client...");

                _ = Task.Run(() => HandleClient(client));
            }
        }

        private Task HandleClient(TcpClient client)
        {
            //Start processing requests from the client
            BufferedStream stream = new BufferedStream(client.GetStream(), 8192);
            ProcessRequest(stream, stream);
                
            //Connection was closed
            stream.Dispose();
            client.Dispose();
            Console.WriteLine("Client disconnected.");
            return Task.CompletedTask;
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