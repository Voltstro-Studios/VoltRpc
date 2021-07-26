﻿using System;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     <see cref="Exception"/> related to when a method fails on the host
    /// </summary>
    public class MethodInvokeFailedException : Exception
    {
        /// <inheritdoc />
        public override string StackTrace { get; }

        /// <summary>
        ///     Creates a new <see cref="MethodInvokeFailedException"/> instance
        /// </summary>
        /// <param name="innerExceptionMessage"></param>
        /// <param name="innerStackTrace"></param>
        private MethodInvokeFailedException(string innerExceptionMessage, string innerStackTrace)
            : base(innerExceptionMessage)
        {
            StackTrace = innerStackTrace;
        }

        /// <summary>
        ///     Creates a new <see cref="MethodInvokeFailedException"/> instance
        /// </summary>
        /// <param name="message"></param>
        /// <param name="stackTrace"></param>
        /// <param name="innerExceptionMessage"></param>
        /// <param name="innerStackTrace"></param>
        public MethodInvokeFailedException(string message, string stackTrace, string innerExceptionMessage, string innerStackTrace)
            : base(message, new MethodInvokeFailedException(innerExceptionMessage, innerStackTrace))
        {
            StackTrace = stackTrace;
        }
    }
}