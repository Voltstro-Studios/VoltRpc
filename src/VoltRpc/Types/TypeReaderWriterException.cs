using System;

namespace VoltRpc.Types
{
    /// <summary>
    ///     <see cref="Exception"/> related to something with a type reader writer
    /// </summary>
    public class TypeReaderWriterException : Exception
    {
        /// <inheritdoc />
        public override string StackTrace { get; }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderWriterException"/> instance
        /// </summary>
        /// <param name="innerExceptionMessage"></param>
        /// <param name="innerStackTrace"></param>
        private TypeReaderWriterException(string innerExceptionMessage, string innerStackTrace)
            : base(innerExceptionMessage)
        {
            StackTrace = innerStackTrace;
        }

        /// <summary>
        ///     Creates a new <see cref="TypeReaderWriterException"/> instance
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="innerExceptionMessage"></param>
        /// <param name="innerStackTrace"></param>
        public TypeReaderWriterException(string message, string stackTrace, string innerExceptionMessage, string innerStackTrace)
            : base(message, new TypeReaderWriterException(innerExceptionMessage, innerStackTrace))
        {
            StackTrace = stackTrace;
        }
    }
}