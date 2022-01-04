using System;

namespace VoltRpc.Types;

/// <summary>
///     Exception when there is no type reader/writer available
/// </summary>
public class NoTypeReaderWriterException : Exception
{
    internal NoTypeReaderWriterException()
    {
    }
}