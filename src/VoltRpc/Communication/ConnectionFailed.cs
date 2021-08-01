﻿using System;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     An <see cref="Exception"/> related to a connection failing
    /// </summary>
    public class ConnectionFailed : Exception
    {
        /// <summary>
        ///     Creates a new <see cref="ConnectionFailed"/> instance
        /// </summary>
        /// <param name="message"></param>
        public ConnectionFailed(string message)
            : base(message)
        {
        }
    }
}