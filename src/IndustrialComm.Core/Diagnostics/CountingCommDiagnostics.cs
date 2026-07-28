using System.Threading;
using IndustrialComm.Results;

namespace IndustrialComm.Diagnostics
{
    /// <summary>
    /// Thread-safe counters for tests and simple runtime dashboards.
    /// </summary>
    public sealed class CountingCommDiagnostics : ICommDiagnostics
    {
        private long _bytesSent;
        private long _bytesReceived;
        private int _connectCount;
        private int _disconnectCount;
        private int _failureCount;

        /// <summary>Total bytes reported as sent.</summary>
        public long BytesSentTotal => Interlocked.Read(ref _bytesSent);

        /// <summary>Total bytes reported as received.</summary>
        public long BytesReceivedTotal => Interlocked.Read(ref _bytesReceived);

        /// <summary>Number of connect events.</summary>
        public int ConnectCount => Volatile.Read(ref _connectCount);

        /// <summary>Number of disconnect events.</summary>
        public int DisconnectCount => Volatile.Read(ref _disconnectCount);

        /// <summary>Number of operation failures.</summary>
        public int FailureCount => Volatile.Read(ref _failureCount);

        /// <summary>Last recorded error, if any.</summary>
        public CommError? LastError { get; private set; }

        /// <inheritdoc />
        public void Connected() => Interlocked.Increment(ref _connectCount);

        /// <inheritdoc />
        public void Disconnected(CommError? error)
        {
            Interlocked.Increment(ref _disconnectCount);
            if (error is not null)
            {
                LastError = error;
            }
        }

        /// <inheritdoc />
        public void BytesSent(int count)
        {
            if (count > 0)
            {
                Interlocked.Add(ref _bytesSent, count);
            }
        }

        /// <inheritdoc />
        public void BytesReceived(int count)
        {
            if (count > 0)
            {
                Interlocked.Add(ref _bytesReceived, count);
            }
        }

        /// <inheritdoc />
        public void OperationFailed(CommError error)
        {
            LastError = error;
            Interlocked.Increment(ref _failureCount);
        }
    }
}
