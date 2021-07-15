using System;

namespace VoltRpc.Communication
{
    /// <summary>
    ///     The <see cref="Client"/> sends messages to a <see cref="Host"/>
    /// </summary>
    public abstract class Client : IDisposable
    {
        /// <summary>
        ///     Connects the <see cref="Client"/> to a host
        /// </summary>
        public abstract void Connect();
        
        /// <summary>
        ///     Destroys the <see cref="Client"/> instance
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}