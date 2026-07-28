using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndustrialComm.Diagnostics;
using IndustrialComm.Results;

namespace IndustrialComm.Transport
{
    /// <summary>
    /// <see cref="IByteTransport"/> adapter over any readable/writable <see cref="Stream"/>
    /// (NetworkStream, serial wrappers, MemoryStream for tests, etc.).
    /// </summary>
    public sealed class StreamByteTransport : IByteTransport
    {
        private readonly Stream _stream;
        private readonly bool _leaveOpen;
        private readonly ICommDiagnostics _diagnostics;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private int _connected; // 0 = false, 1 = true
        private int _disposed;

        /// <summary>
        /// Creates a transport around an existing stream.
        /// When <paramref name="assumeConnected"/> is true, <see cref="IsConnected"/> starts as true
        /// (typical for already-open sockets or test streams).
        /// </summary>
        public StreamByteTransport(
            Stream stream,
            bool leaveOpen = false,
            bool assumeConnected = true,
            ICommDiagnostics? diagnostics = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead || !stream.CanWrite)
            {
                throw new ArgumentException("Stream must be readable and writable.", nameof(stream));
            }

            _leaveOpen = leaveOpen;
            _diagnostics = diagnostics ?? NullCommDiagnostics.Instance;
            _connected = assumeConnected ? 1 : 0;
        }

        /// <inheritdoc />
        public bool IsConnected => Volatile.Read(ref _connected) == 1 && Volatile.Read(ref _disposed) == 0;

        /// <inheritdoc />
        public ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (Interlocked.Exchange(ref _connected, 1) == 0)
            {
                _diagnostics.Connected();
            }

            return default;
        }

        /// <inheritdoc />
        public async ValueTask DisconnectAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (Interlocked.Exchange(ref _connected, 0) == 1)
            {
                try
                {
                    await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var error = CommError.FromException(ex, CommErrorCode.DisconnectFailed);
                    _diagnostics.Disconnected(error);
                    throw;
                }

                _diagnostics.Disconnected(null);
            }
        }

        /// <inheritdoc />
        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected)
            {
                throw new InvalidOperationException("Transport is not connected.");
            }

            try
            {
#if NETSTANDARD2_0
                var array = new byte[buffer.Length];
                var read = await _stream.ReadAsync(array, 0, array.Length, cancellationToken).ConfigureAwait(false);
                if (read > 0)
                {
                    array.AsSpan(0, read).CopyTo(buffer.Span);
                    _diagnostics.BytesReceived(read);
                }
                else
                {
                    MarkDisconnected(new CommError(
                        CommErrorCode.ConnectionClosed,
                        "Remote peer closed the connection."));
                }

                return read;
#else
                var read = await _stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (read > 0)
                {
                    _diagnostics.BytesReceived(read);
                }
                else
                {
                    MarkDisconnected(new CommError(
                        CommErrorCode.ConnectionClosed,
                        "Remote peer closed the connection."));
                }

                return read;
#endif
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var error = CommError.FromException(ex, CommErrorCode.ReadFailed);
                MarkDisconnected(error);
                throw;
            }
        }

        /// <inheritdoc />
        public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            if (!IsConnected)
            {
                throw new InvalidOperationException("Transport is not connected.");
            }

            await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
#if NETSTANDARD2_0
                var array = buffer.ToArray();
                await _stream.WriteAsync(array, 0, array.Length, cancellationToken).ConfigureAwait(false);
#else
                await _stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
#endif
                if (buffer.Length > 0)
                {
                    _diagnostics.BytesSent(buffer.Length);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var error = CommError.FromException(ex, CommErrorCode.WriteFailed);
                MarkDisconnected(error);
                throw;
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            Volatile.Write(ref _connected, 0);
            try
            {
                if (!_leaveOpen)
                {
#if NETSTANDARD2_0
                    _stream.Dispose();
#else
                    await _stream.DisposeAsync().ConfigureAwait(false);
#endif
                }
            }
            finally
            {
                _writeLock.Dispose();
            }

#if NETSTANDARD2_0
            await Task.CompletedTask.ConfigureAwait(false);
#endif
        }

        private void MarkDisconnected(CommError error)
        {
            if (Interlocked.Exchange(ref _connected, 0) == 1)
            {
                _diagnostics.Disconnected(error);
                _diagnostics.OperationFailed(error);
            }
        }

        private void ThrowIfDisposed()
        {
            if (Volatile.Read(ref _disposed) != 0)
            {
                throw new ObjectDisposedException(nameof(StreamByteTransport));
            }
        }
    }
}
