namespace VoltRpc.Logging
{
    /// <summary>
    ///     Provides an interface for logging
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///     Logging level
        /// </summary>
        public LogVerbosity LogVerbosity { get; set; }
        
        /// <summary>
        ///     Log a debug message
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message);

        /// <summary>
        ///     Log a info message
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message);

        /// <summary>
        ///     Log a waring message
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message);

        /// <summary>
        ///     Log a error message
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message);
    }
}