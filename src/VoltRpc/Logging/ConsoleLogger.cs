using System;

namespace VoltRpc.Logging
{
    /// <summary>
    ///     Logger for <see cref="System.Console" />
    /// </summary>
    public sealed class ConsoleLogger : ILogger
    {
        /// <summary>
        ///     Creates a new <see cref="ConsoleLogger" /> instance
        /// </summary>
        /// <param name="logVerbosity"></param>
        public ConsoleLogger(LogVerbosity logVerbosity = LogVerbosity.Info)
        {
            LogVerbosity = logVerbosity;
        }

        /// <inheritdoc />
        public LogVerbosity LogVerbosity { get; set; }

        /// <inheritdoc />
        public void Debug(string message)
        {
            if (LogVerbosity <= LogVerbosity.Debug)
                Console.WriteLine($"[DEBUG] {message}");
        }

        /// <inheritdoc />
        public void Info(string message)
        {
            if (LogVerbosity <= LogVerbosity.Info)
                Console.WriteLine($"[INFO] {message}");
        }

        /// <inheritdoc />
        public void Warn(string message)
        {
            if (LogVerbosity <= LogVerbosity.Warn)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARN] {message}");
                Console.ForegroundColor = currentColor;
            }
        }

        /// <inheritdoc />
        public void Error(string message)
        {
            if (LogVerbosity <= LogVerbosity.Error)
            {
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {message}");
                Console.ForegroundColor = currentColor;
            }
        }
    }
}