using System;

namespace IndustrialComm.Framing
{
    /// <summary>
    /// Detects frames of a constant byte length.
    /// </summary>
    public sealed class FixedLengthFrameDetector : IFrameDetector
    {
        private readonly int _frameLength;

        /// <summary>
        /// Creates a detector for fixed-length frames.
        /// </summary>
        /// <param name="frameLength">Exact frame size in bytes; must be greater than zero.</param>
        public FixedLengthFrameDetector(int frameLength)
        {
            if (frameLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(frameLength));
            }

            _frameLength = frameLength;
        }

        /// <inheritdoc />
        public FrameDetectStatus TryDetect(ReadOnlySpan<byte> buffer, out int frameLength, out int bytesToSkip)
        {
            frameLength = 0;
            bytesToSkip = 0;

            if (buffer.Length < _frameLength)
            {
                return FrameDetectStatus.NeedMoreData;
            }

            frameLength = _frameLength;
            return FrameDetectStatus.FrameReady;
        }
    }
}
