using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace VoltRpc.IO;
/*
 * A lot of this code comes from Mirror's NetworkWriter:
 * https://github.com/vis2k/Mirror/blob/298435001afe407c8ec76bc4268f0cb1caff4f96/Assets/Mirror/Runtime/NetworkWriter.cs
 */

/// <summary>
///     A buffered writer for a <see cref="Stream" />
/// </summary>
public class BufferedWriter : IDisposable
{
    /// <summary>
    ///     Max length for a <see cref="string" />
    /// </summary>
    public const int MaxStringLength = 1024 * 32;
    
    /// <summary>
    ///     Output <see cref="Stream" />
    /// </summary>
    protected readonly Stream OutputStream;

    /// <summary>
    ///     Internal access to the underlining <see cref="UTF8Encoding"/> for <see cref="string"/>s
    /// </summary>
    internal readonly UTF8Encoding encoding;
    
    /// <summary>
    ///     Internal access to the underlining buffer
    /// </summary>
    internal byte[] buffer;
    
    private readonly byte[] stringBuffer;

    /// <summary>
    ///     Creates a new <see cref="BufferedWriter" /> instance
    /// </summary>
    /// <param name="output"></param>
    /// <param name="bufferSize"></param>
    public BufferedWriter(Stream output, int bufferSize = 8000)
    {
        OutputStream = output;
        encoding = new UTF8Encoding(false, true);
        stringBuffer = new byte[MaxStringLength];
        buffer = IoUtils.CreateBuffer(bufferSize);
    }

    /// <summary>
    ///     The current position of the buffer
    /// </summary>
    public int Position { get; internal set; }

    /// <summary>
    ///     The length of the buffer
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => buffer.Length;
    }

    /// <summary>
    ///     You may need to override this if your <see cref="Stream" /> requires it
    /// </summary>
    protected virtual long OutputStreamPosition { get; set; }

    /// <summary>
    ///     Reset position
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        OutputStreamPosition = 0;
        Position = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe void WriteBlittable<T>(T value)
        where T : unmanaged
    {
        int size = sizeof(T);
        
        EnsureCapacity(Position + size);
        fixed (byte* ptr = &buffer[Position])
        {
            *(T*)ptr = value;
        }

        Position += size;
    }

    /// <summary>
    ///     Writes a <see cref="byte" />
    /// </summary>
    /// <param name="value">The value to write</param>
    public void WriteByte(byte value)
    {
        WriteBlittable(value);
    }

    /// <summary>
    ///     Writes an array of <see cref="byte" />s
    /// </summary>
    /// <param name="bytesBuffer"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    public void WriteBytes(byte[] bytesBuffer, int offset, int count)
    {
        EnsureCapacity(Position + count);
        Array.ConstrainedCopy(bytesBuffer, offset, buffer, Position, count);
        Position += count;
    }

    /// <summary>
    ///     Writes a <see cref="string" />
    /// </summary>
    /// <param name="value"></param>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public void WriteString(string value)
    {
        //Null support
        if (value == null)
        {
            this.WriteUShort(0);
            return;
        }

        //Write to string buffer
        int size = encoding.GetBytes(value, 0, value.Length, stringBuffer, 0);

        //Check if within max size
        if (size >= MaxStringLength)
            throw new IndexOutOfRangeException($"Cannot write string larger then {MaxStringLength}!");

        //Write size and bytes
        this.WriteUShort(checked((ushort) (size + 1)));
        WriteBytes(stringBuffer, 0, size);
    }

    /// <summary>
    ///     Writes the buffer to the out <see cref="Stream" />
    /// </summary>
    public void Flush()
    {
        OutputStream.Write(buffer, 0, Position);
        OutputStream.Flush();
        OutputStreamPosition = 0;
        Reset();
    }

    /// <summary>
    ///     Ensures the buffer's capacity is large enough to write the size
    /// </summary>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EnsureCapacity(int value)
    {
        if (buffer.Length < value)
        {
            int capacity = Math.Max(value, buffer.Length * 2);

            //Create new buffer and copy the contents of the old one to it
            byte[] newBuffer = IoUtils.CreateBuffer(capacity);
            Array.Copy(buffer, newBuffer, buffer.Length);
            buffer = newBuffer;
        }
    }

    #region Destroy

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
}