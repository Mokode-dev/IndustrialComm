namespace IndustrialComm.Results
{
    /// <summary>
    /// Stable industrial communication error codes shared across the ecosystem.
    /// Protocol packages may map vendor-specific faults into these codes.
    /// </summary>
    public enum CommErrorCode
    {
        /// <summary>Unspecified or unknown failure.</summary>
        Unknown = 0,

        /// <summary>Operation timed out waiting for a response or I/O.</summary>
        Timeout = 1,

        /// <summary>Transport is not connected.</summary>
        Disconnected = 2,

        /// <summary>Transport failed while connecting.</summary>
        ConnectFailed = 3,

        /// <summary>Transport failed while disconnecting.</summary>
        DisconnectFailed = 4,

        /// <summary>Read from the transport failed.</summary>
        ReadFailed = 5,

        /// <summary>Write to the transport failed.</summary>
        WriteFailed = 6,

        /// <summary>Checksum or CRC validation failed.</summary>
        ChecksumMismatch = 7,

        /// <summary>Received frame is incomplete or malformed.</summary>
        FramingError = 8,

        /// <summary>Buffer capacity was exceeded.</summary>
        BufferOverflow = 9,

        /// <summary>Protocol-level negative response or exception.</summary>
        ProtocolError = 10,

        /// <summary>Requested address or quantity is invalid.</summary>
        InvalidAddress = 11,

        /// <summary>Argument or configuration is invalid.</summary>
        InvalidArgument = 12,

        /// <summary>Operation was cancelled via <see cref="System.Threading.CancellationToken"/>.</summary>
        Cancelled = 13,

        /// <summary>Retry budget was exhausted.</summary>
        RetryExhausted = 14,

        /// <summary>Remote peer closed the connection.</summary>
        ConnectionClosed = 15,

        /// <summary>Operation is not supported in the current state or configuration.</summary>
        NotSupported = 16,

        /// <summary>Device reported bad quality or unavailable data.</summary>
        BadQuality = 17,
    }
}
