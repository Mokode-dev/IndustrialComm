using System;
using System.Diagnostics;

namespace IndustrialComm.Results
{
    /// <summary>
    /// Immutable description of a failed industrial communication operation.
    /// Prefer returning <see cref="CommError"/> via <see cref="CommResult"/> for expected failures.
    /// </summary>
    [DebuggerDisplay("{Code}: {Message}")]
    public sealed class CommError
    {
        /// <summary>
        /// Creates a new error instance.
        /// </summary>
        /// <param name="code">Machine-readable error code.</param>
        /// <param name="message">Human-readable message.</param>
        /// <param name="exception">Optional underlying exception.</param>
        /// <param name="detail">Optional protocol or vendor detail.</param>
        public CommError(CommErrorCode code, string message, Exception? exception = null, string? detail = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Error message is required.", nameof(message));
            }

            Code = code;
            Message = message;
            Exception = exception;
            Detail = detail;
        }

        /// <summary>Machine-readable error code.</summary>
        public CommErrorCode Code { get; }

        /// <summary>Human-readable message.</summary>
        public string Message { get; }

        /// <summary>Optional underlying exception (programming or I/O fault).</summary>
        public Exception? Exception { get; }

        /// <summary>Optional extra detail (hex dump, vendor code, etc.).</summary>
        public string? Detail { get; }

        /// <summary>Creates a timeout error.</summary>
        public static CommError Timeout(string message = "The operation timed out.", Exception? exception = null)
            => new CommError(CommErrorCode.Timeout, message, exception);

        /// <summary>Creates a disconnected error.</summary>
        public static CommError Disconnected(string message = "The transport is not connected.", Exception? exception = null)
            => new CommError(CommErrorCode.Disconnected, message, exception);

        /// <summary>Creates a checksum mismatch error.</summary>
        public static CommError ChecksumMismatch(string message = "Checksum validation failed.", string? detail = null)
            => new CommError(CommErrorCode.ChecksumMismatch, message, detail: detail);

        /// <summary>Creates a framing error.</summary>
        public static CommError Framing(string message, string? detail = null)
            => new CommError(CommErrorCode.FramingError, message, detail: detail);

        /// <summary>Creates a cancelled error.</summary>
        public static CommError Cancelled(string message = "The operation was cancelled.", Exception? exception = null)
            => new CommError(CommErrorCode.Cancelled, message, exception);

        /// <summary>Creates an invalid-argument error.</summary>
        public static CommError InvalidArgument(string message, string? detail = null)
            => new CommError(CommErrorCode.InvalidArgument, message, detail: detail);

        /// <summary>Creates an error from an unexpected exception.</summary>
        public static CommError FromException(Exception exception, CommErrorCode code = CommErrorCode.Unknown)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (exception is OperationCanceledException)
            {
                return Cancelled(exception.Message, exception);
            }

            return new CommError(code, exception.Message, exception);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Detail is null
                ? $"{Code}: {Message}"
                : $"{Code}: {Message} ({Detail})";
        }
    }
}
