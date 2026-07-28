using System;
using System.Threading;
using System.Threading.Tasks;

namespace IndustrialComm.Transport
{
    /// <summary>
    /// Protocol-agnostic full-duplex byte stream used by IndustrialComm protocol packages.
    /// Implementations may wrap TCP, serial ports, named pipes, or in-memory test doubles.
    /// </summary>
    public interface IByteTransport : IAsyncDisposable
    {
        /// <summary>True when the transport considers itself connected and usable.</summary>
        bool IsConnected { get; }

        /// <summary>Establishes the underlying connection.</summary>
        ValueTask ConnectAsync(CancellationToken cancellationToken = default);

        /// <summary>Gracefully closes the underlying connection.</summary>
        ValueTask DisconnectAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads bytes into <paramref name="buffer"/>.
        /// Returns the number of bytes read; 0 typically means the remote side closed the stream.
        /// </summary>
        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);

        /// <summary>Writes all bytes from <paramref name="buffer"/>.</summary>
        ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
    }
}
