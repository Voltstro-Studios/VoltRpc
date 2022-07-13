using System;

namespace VoltRpc.Types;

/// <summary>
///     Exception when there is no type reader/writer available
/// </summary>
public sealed class NoTypeReaderWriterException : Exception
{
    internal NoTypeReaderWriterException(string message)
        : base(message)
    {
    }
}