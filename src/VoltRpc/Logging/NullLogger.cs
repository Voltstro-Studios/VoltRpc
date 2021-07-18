namespace VoltRpc.Logging
{
    /// <summary>
    ///     Null <see cref="ILogger"/>
    /// </summary>
    public sealed class NullLogger : ILogger
    {
        /// <inheritdoc/>
        public LogVerbosity LogVerbosity { get; set; }

        /// <inheritdoc/>
        public void Debug(string message)
        {
        }

        /// <inheritdoc/>
        public void Info(string message)
        {
        }

        /// <inheritdoc/>
        public void Warn(string message)
        {
        }

        /// <inheritdoc/>
        public void Error(string message)
        {
        }
    }
}