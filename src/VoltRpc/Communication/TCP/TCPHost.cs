using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VoltRpc.Logging;

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
        /// <param name="endPoint">The <see cref="IPEndPoint"/> to listen on</param>
        /// <param name="logger">The <see cref="ILogger"/> to use. Will default to <see cref="NullLogger"/> if null</param>
        /// <param name="receiveTimeout">How long until timeout from receiving</param>
        /// <param name="sendTimeout">How long until timeout from sending</param>
        public TCPHost(IPEndPoint endPoint, ILogger logger = null, int receiveTimeout = 600000, int sendTimeout = 600000)
        : base(logger)
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
            Logger.Debug("TCP host now listening...");
            
            while (isRunning)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                client.ReceiveTimeout = receiveTimeout;
                client.SendTimeout = sendTimeout;
                
                Logger.Debug("Accepted client...");

                _ = Task.Run(() => HandleClient(client));
            }
        }

        private Task HandleClient(TcpClient client)
        {
            //Start processing requests from the client
            Stream stream = client.GetStream();
            ProcessRequest(stream, stream);
                
            //Connection was closed
            stream.Dispose();
            client.Dispose();
            Logger.Debug("Client disconnected.");
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