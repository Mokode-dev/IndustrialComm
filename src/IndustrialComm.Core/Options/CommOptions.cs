using System;

namespace IndustrialComm.Options
{
    /// <summary>
    /// Shared communication options used as a base for protocol-specific option types.
    /// </summary>
    public class CommOptions
    {
        /// <summary>Default I/O timeout. Default: 3 seconds.</summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>Maximum frame size accepted by framing buffers. Default: 256 KB.</summary>
        public int MaxFrameLength { get; set; } = 256 * 1024;

        /// <summary>Receive buffer capacity. Default: 256 KB.</summary>
        public int ReceiveBufferSize { get; set; } = 256 * 1024;

        /// <summary>
        /// Validates option values. Throws <see cref="ArgumentOutOfRangeException"/> when invalid.
        /// </summary>
        public virtual void Validate()
        {
            if (Timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(Timeout), "Timeout must be greater than zero.");
            }

            if (MaxFrameLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(MaxFrameLength), "MaxFrameLength must be greater than zero.");
            }

            if (ReceiveBufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ReceiveBufferSize), "ReceiveBufferSize must be greater than zero.");
            }

            if (ReceiveBufferSize < MaxFrameLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ReceiveBufferSize),
                    "ReceiveBufferSize should be greater than or equal to MaxFrameLength.");
            }
        }
    }
}
