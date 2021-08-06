using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

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
            Task<TcpClient> connectTask = client
                .ConnectAsync(endPoint.Address, endPoint.Port).ContinueWith(task => task.IsFaulted ? 
                        throw new ConnectionFailed("The TCP client failed to connect to a host for some reason!") : client, 
                    TaskContinuationOptions.ExecuteSynchronously);
            Task<TcpClient> timeoutTask = Task.Delay(connectionTimeout)
                .ContinueWith<TcpClient>(task => throw new TimeoutException(), 
                    TaskContinuationOptions.ExecuteSynchronously);
            Task<TcpClient> result = Task.WhenAny(connectTask, timeoutTask).Unwrap();

            try
            {
                result.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Any(x => x is TimeoutException))
                    throw new TimeoutException();

                throw new ConnectionFailed("The TCP client failed to connect to a host for some reason!");
            }
            
            //Backup
            if(result.Result == null)
                throw new ConnectionFailed("The TCP client failed to connect to a host for some reason!");

            clientStream = result.Result.GetStream();
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