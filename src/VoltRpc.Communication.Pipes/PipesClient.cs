using System.IO.Pipes;

namespace VoltRpc.Communication.Pipes
{
    /// <summary>
    ///     <see cref="Client" /> using named pipes
    /// </summary>
    public class PipesClient : Client
    {
        /// <summary>
        ///     Default connection timeout time
        /// </summary>
        public const int DefaultConnectionTimeout = 7000;

        private readonly int connectionTimeout;

        private readonly NamedPipeClientStream namedPipeClientStream;

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="server">The server to connect to</param>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="connectionTimeout">The timeout for connection</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string server, string pipeName, int connectionTimeout, int bufferSize = DefaultBufferSize)
            : base(bufferSize)
        {
            this.connectionTimeout = connectionTimeout;
            namedPipeClientStream = new NamedPipeClientStream(server, pipeName, PipeDirection.InOut);
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="server">The server to connect to</param>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string server, string pipeName, int bufferSize = DefaultBufferSize)
            : this(server, pipeName, DefaultConnectionTimeout, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="connectionTimeout">The timeout for connection</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string pipeName, int connectionTimeout, int bufferSize = DefaultBufferSize)
            : this(".", pipeName, connectionTimeout, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string pipeName, int bufferSize = DefaultBufferSize)
            : this(".", pipeName, bufferSize)
        {
        }

        /// <inheritdoc />
        /// <exception cref="ConnectionFailed">Thrown if an unknown error occurs while connecting.</exception>
        public override void Connect()
        {
            namedPipeClientStream.Connect(connectionTimeout);

            if (!namedPipeClientStream.IsConnected)
                throw new ConnectionFailed("Failed to connect to a pipes host!");

            IsConnectedInternal = true;
            Initialize(namedPipeClientStream, namedPipeClientStream);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            namedPipeClientStream.Dispose();
            IsConnectedInternal = false;
        }
    }
}