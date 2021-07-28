using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using VoltRpc.Logging;

namespace VoltRpc.Communication.Pipes
{
    /// <summary>
    ///     <see cref="Host" /> using named pipes
    /// </summary>
    public class PipesHost : Host
    {
        /// <summary>
        ///     The default max connection
        /// </summary>
        public const int DefaultMaxConnections = 128;

        private readonly int maxConnections;

        private readonly string pipeName;

        private bool isRunning;

        /// <summary>
        ///     Creates a new <see cref="PipesHost" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="maxConnections">The max amount of connections to handle</param>
        /// <param name="logger">The <see cref="ILogger" /> to use</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesHost(string pipeName, int maxConnections = DefaultMaxConnections, ILogger logger = null,
            int bufferSize = 8000)
            : base(logger, bufferSize)
        {
            this.pipeName = pipeName;
            this.maxConnections = maxConnections;
        }

        /// <summary>
        ///     Creates a new <see cref="PipesHost" /> instance
        /// </summary>
        /// <param name="pipeName">The name of the pipe</param>
        /// <param name="logger">The <see cref="ILogger" /> to use</param>
        /// <param name="bufferSize">The size of the buffers</param>
        public PipesHost(string pipeName, ILogger logger = null, int bufferSize = 8000)
            : this(pipeName, DefaultMaxConnections, logger, bufferSize)
        {
        }

        /// <inheritdoc />
        public override Task StartListening()
        {
            Task.Factory.StartNew(SeverLoop, TaskCreationOptions.LongRunning);
            return Task.CompletedTask;
        }

        private void SeverLoop()
        {
            Logger.Debug("Named Pipes host now listening...");

            isRunning = true;
            while (isRunning)
                try
                {
                    NamedPipeServerStream serverStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut,
                        maxConnections);
                    serverStream.WaitForConnection();
                    _ = Task.Run(() => HandleClient(serverStream));
                }
                catch (Exception ex)
                {
                    Logger.Error($"An error occured while handling incoming pipes connections! {ex}");
                }
        }

        private Task HandleClient(NamedPipeServerStream stream)
        {
            Logger.Debug("Accepted client...");
            ProcessRequest(stream, stream);
            stream.Dispose();
            Logger.Debug("Client disconnected.");
            return Task.CompletedTask;
        }
    }
}