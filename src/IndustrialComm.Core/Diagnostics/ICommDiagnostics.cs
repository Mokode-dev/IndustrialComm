using IndustrialComm.Results;

namespace IndustrialComm.Diagnostics
{
    /// <summary>
    /// Lightweight diagnostics sink for transports and protocol clients.
    /// Implementations may bridge to EventSource, OpenTelemetry, metrics, or logs.
    /// </summary>
    public interface ICommDiagnostics
    {
        /// <summary>Transport became connected.</summary>
        void Connected();

        /// <summary>Transport became disconnected; <paramref name="error"/> is null on graceful close.</summary>
        void Disconnected(CommError? error);

        /// <summary>Bytes successfully written to the wire.</summary>
        void BytesSent(int count);

        /// <summary>Bytes successfully read from the wire.</summary>
        void BytesReceived(int count);

        /// <summary>An operation failed with the given error.</summary>
        void OperationFailed(CommError error);
    }
}
