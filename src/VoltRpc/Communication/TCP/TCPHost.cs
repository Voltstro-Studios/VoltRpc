﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VoltRpc.Logging;

namespace VoltRpc.Communication.TCP
{
    /// <summary>
    ///     A <see cref="Host" /> that uses TCP to communicate
    /// </summary>
    public sealed class TCPHost : Host
    {
        /// <summary>
        ///     Default receive timeout time
        /// </summary>
        public const int DefaultReceiveTimeout = 600000;

        /// <summary>
        ///     Default send timeout time
        /// </summary>
        public const int DefaultSendTimeout = 600000;

        private const int ListenerBacklog = 8192;

        private readonly TcpListener listener;

        private readonly int receiveTimeout;
        private readonly int sendTimeout;

        /// <summary>
        ///     Creates a new <see cref="TCPHost" /> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint" /> to listen on</param>
        /// <param name="logger">The <see cref="ILogger" /> to use. Will default to <see cref="NullLogger" /> if null</param>
        /// <param name="bufferSize">The initial size of the buffers</param>
        /// <param name="receiveTimeout">How long until timeout from receiving</param>
        /// <param name="sendTimeout">How long until timeout from sending</param>
        /// <exception cref="ArgumentOutOfRangeException">Will throw if the buffer size is less then 16</exception>
        public TCPHost(IPEndPoint endPoint, ILogger logger, int bufferSize,
            int receiveTimeout = DefaultReceiveTimeout, int sendTimeout = DefaultSendTimeout)
            : base(logger, bufferSize)
        {
            listener = new TcpListener(endPoint);
            this.receiveTimeout = receiveTimeout;
            this.sendTimeout = sendTimeout;
        }

        /// <summary>
        ///     Creates a new <see cref="TCPHost" /> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint" /> to listen on</param>
        /// <param name="bufferSize">The initial size of the buffers</param>
        /// <param name="receiveTimeout">How long until timeout from receiving</param>
        /// <param name="sendTimeout">How long until timeout from sending</param>
        public TCPHost(IPEndPoint endPoint, int bufferSize,
            int receiveTimeout = DefaultReceiveTimeout, int sendTimeout = DefaultSendTimeout)
            : this(endPoint, null, bufferSize, receiveTimeout, sendTimeout)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="TCPHost" /> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint" /> to listen on</param>
        /// <param name="logger">The <see cref="ILogger" /> to use. Will default to <see cref="NullLogger" /> if null</param>
        /// <param name="receiveTimeout">How long until timeout from receiving</param>
        /// <param name="sendTimeout">How long until timeout from sending</param>
        public TCPHost(IPEndPoint endPoint, ILogger logger, int receiveTimeout, int sendTimeout)
            : this(endPoint, logger, DefaultBufferSize, receiveTimeout, sendTimeout)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="TCPHost" /> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint" /> to listen on</param>
        /// <param name="receiveTimeout">How long until timeout from receiving</param>
        /// <param name="sendTimeout">How long until timeout from sending</param>
        public TCPHost(IPEndPoint endPoint, int receiveTimeout, int sendTimeout)
            : this(endPoint, null, DefaultBufferSize, receiveTimeout, sendTimeout)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="TCPHost" /> instance
        /// </summary>
        /// <param name="endPoint">The <see cref="IPEndPoint" /> to listen on</param>
        /// <param name="logger">The <see cref="ILogger" /> to use. Will default to <see cref="NullLogger" /> if null</param>
        public TCPHost(IPEndPoint endPoint, ILogger logger = null)
            : this(endPoint, logger, DefaultBufferSize)
        {
        }

        /// <inheritdoc />
        public override async Task StartListening()
        {
            IsRunning = true;
            listener.Start(ListenerBacklog);
            Logger.Debug("TCP host now listening...");

            while (IsRunning)
            {
                if (ConnectionCount >= MaxConnectionsCount)
                {
                    listener.Stop();
                    continue;
                }
                
                if(!listener.Server.IsBound)
                    listener.Start(ListenerBacklog);
                
                Logger.Debug("TCP host is listening for a connection...");
                TcpClient client = await listener.AcceptTcpClientAsync();
                client.ReceiveTimeout = receiveTimeout;
                client.SendTimeout = sendTimeout;

                Logger.Debug("Accepted client...");
                ConnectionCount++;

                _ = Task.Run(() => HandleClient(client));
            }
            
            Logger.Debug("TCP host has stopped listening.");
        }

        private Task HandleClient(TcpClient client)
        {
            //Start processing requests from the client
            Stream stream = client.GetStream();
            ProcessRequest(stream, stream);

            //Connection was closed
            stream.Dispose();
            client.Dispose();
            ConnectionCount--;
            Logger.Debug("Client disconnected.");
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            listener.Stop();
        }
    }
}