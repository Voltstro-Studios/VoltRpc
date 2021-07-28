using System.IO.Pipes;

namespace VoltRpc.Communication.Pipes
{
    /// <summary>
    ///     <see cref="Client" /> using named pipes
    /// </summary>
    public class PipesClient : Client
    {
        private readonly int connectionTimeout;

        private readonly NamedPipeClientStream namedPipeClientStream;

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="server">The server to connect to</param>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="connectionTimeout">The timeout for connection</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string server, string pipeName, int connectionTimeout = 7000, int bufferSize = 8000)
            : base(bufferSize)
        {
            this.connectionTimeout = connectionTimeout;
            namedPipeClientStream = new NamedPipeClientStream(server, pipeName, PipeDirection.InOut);
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="connectionTimeout">The timeout for connection</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string pipeName, int connectionTimeout = 7000, int bufferSize = 8000)
            : this(".", pipeName, connectionTimeout, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="server">The server to connect to</param>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string server, string pipeName, int bufferSize = 8000)
            : this(server, pipeName, 7000, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesClient(string pipeName, int bufferSize = 8000)
            : this(".", pipeName, 7000, bufferSize)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="PipesClient" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        public PipesClient(string pipeName)
            : this(".", pipeName, 7000)
        {
        }

        /// <inheritdoc />
        public override void Connect()
        {
            namedPipeClientStream.ConnectAsync(connectionTimeout);
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