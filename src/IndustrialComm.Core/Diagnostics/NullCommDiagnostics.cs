using IndustrialComm.Results;

namespace IndustrialComm.Diagnostics
{
    /// <summary>
    /// No-op diagnostics sink used as the default.
    /// </summary>
    public sealed class NullCommDiagnostics : ICommDiagnostics
    {
        /// <summary>Shared instance.</summary>
        public static NullCommDiagnostics Instance { get; } = new NullCommDiagnostics();

        private NullCommDiagnostics()
        {
        }

        /// <inheritdoc />
        public void Connected()
        {
        }

        /// <inheritdoc />
        public void Disconnected(CommError? error)
        {
        }

        /// <inheritdoc />
        public void BytesSent(int count)
        {
        }

        /// <inheritdoc />
        public void BytesReceived(int count)
        {
        }

        /// <inheritdoc />
        public void OperationFailed(CommError error)
        {
        }
    }
}
