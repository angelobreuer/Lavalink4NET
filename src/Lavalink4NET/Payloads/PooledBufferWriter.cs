namespace Lavalink4NET.Payloads;

using System;
using System.Buffers;
using System.Threading;

internal sealed class PooledBufferWriter : IBufferWriter<byte>, IDisposable
{
    private ArrayPool<byte>? _arrayPool; // null = disposed
    private byte[]? _buffer;
    private int _bytesWritten;

    public PooledBufferWriter()
        : this(ArrayPool<byte>.Shared)
    {
    }

    public PooledBufferWriter(ArrayPool<byte> arrayPool)
    {
        _arrayPool = arrayPool;
    }

    public int WrittenCount
    {
        get
        {
            EnsureNotDisposed();
            return _bytesWritten;
        }
    }

    public ReadOnlyMemory<byte> WrittenMemory
    {
        get
        {
            EnsureNotDisposed();
            return _buffer is null ? default : _buffer.AsMemory(0, _bytesWritten);
        }
    }

    public ArraySegment<byte> WrittenSegment
    {
        get
        {
            EnsureNotDisposed();
            return _buffer is null ? default : new ArraySegment<byte>(_buffer, 0, _bytesWritten);
        }
    }

    public ReadOnlySpan<byte> WrittenSpan => WrittenMemory.Span;

    /// <inheritdoc/>
    public void Advance(int count)
    {
        EnsureNotDisposed();

        if (_buffer is null)
        {
            throw new InvalidOperationException("No buffer was allocated for this buffer writer.");
        }

        // TODO: more checks
        _bytesWritten += count;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_arrayPool is null)
        {
            return;
        }

        var buffer = Interlocked.Exchange(ref _buffer, null);

        if (buffer is not null)
        {
            _arrayPool.Return(buffer);
        }

        _arrayPool = null;
    }

    /// <inheritdoc/>
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        EnsureNotDisposed();

        if (sizeHint is 0)
        {
            sizeHint = 1;
        }

        if (_buffer is null)
        {
            _buffer = _arrayPool!.Rent(sizeHint);
        }

        if (_buffer.Length - _bytesWritten < sizeHint)
        {
            var newBuffer = _arrayPool!.Rent(sizeHint + _bytesWritten);
            _buffer.AsSpan(0, _bytesWritten).CopyTo(newBuffer);
            _arrayPool.Return(_buffer);
            _buffer = newBuffer;
        }

        return _buffer.AsMemory(_bytesWritten);
    }

    /// <inheritdoc/>
    public Span<byte> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;

    public void Reset()
    {
        EnsureNotDisposed();

        var buffer = Interlocked.Exchange(ref _buffer, null);

        if (buffer is not null)
        {
            _arrayPool!.Return(buffer);
        }
    }

    private void EnsureNotDisposed()
    {
        if (_arrayPool is null)
        {
            throw new ObjectDisposedException(nameof(PooledBufferWriter));
        }
    }
}