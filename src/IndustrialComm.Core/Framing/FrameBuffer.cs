using System;
using IndustrialComm.Results;

namespace IndustrialComm.Framing
{
    /// <summary>
    /// Circular-style receive buffer that accumulates bytes and extracts complete frames
    /// using an <see cref="IFrameDetector"/>.
    /// </summary>
    public sealed class FrameBuffer
    {
        private readonly byte[] _buffer;
        private readonly IFrameDetector _detector;
        private int _length;

        /// <summary>
        /// Creates a frame buffer.
        /// </summary>
        /// <param name="capacity">Maximum buffered bytes before overflow.</param>
        /// <param name="detector">Frame detection strategy.</param>
        public FrameBuffer(int capacity, IFrameDetector detector)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _buffer = new byte[capacity];
            _detector = detector ?? throw new ArgumentNullException(nameof(detector));
        }

        /// <summary>Number of bytes currently buffered.</summary>
        public int Length => _length;

        /// <summary>Total buffer capacity.</summary>
        public int Capacity => _buffer.Length;

        /// <summary>Clears all buffered data.</summary>
        public void Clear() => _length = 0;

        /// <summary>
        /// Appends received bytes to the buffer.
        /// </summary>
        /// <returns>Success, or <see cref="CommErrorCode.BufferOverflow"/> when capacity is exceeded.</returns>
        public CommResult Write(ReadOnlySpan<byte> data)
        {
            if (data.IsEmpty)
            {
                return CommResult.Success;
            }

            if (_length + data.Length > _buffer.Length)
            {
                return CommResult.Failure(
                    CommErrorCode.BufferOverflow,
                    $"Frame buffer capacity {_buffer.Length} exceeded (have {_length}, writing {data.Length}).");
            }

            data.CopyTo(_buffer.AsSpan(_length));
            _length += data.Length;
            return CommResult.Success;
        }

        /// <summary>
        /// Attempts to extract the next complete frame into <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">Destination for frame bytes.</param>
        /// <param name="bytesWritten">Number of frame bytes written.</param>
        /// <returns>
        /// Success with a frame, NeedMoreData-style success with 0 bytes, or framing/overflow failure.
        /// When no complete frame is available, returns success with <paramref name="bytesWritten"/> = 0.
        /// </returns>
        public CommResult TryReadFrame(Span<byte> destination, out int bytesWritten)
        {
            bytesWritten = 0;

            while (true)
            {
                if (_length == 0)
                {
                    return CommResult.Success;
                }

                var status = _detector.TryDetect(_buffer.AsSpan(0, _length), out var frameLength, out var bytesToSkip);
                switch (status)
                {
                    case FrameDetectStatus.NeedMoreData:
                        return CommResult.Success;

                    case FrameDetectStatus.Skip:
                        if (bytesToSkip <= 0)
                        {
                            return CommResult.Failure(CommError.Framing("Frame detector requested skip of zero bytes."));
                        }

                        Discard(Math.Min(bytesToSkip, _length));
                        continue;

                    case FrameDetectStatus.Invalid:
                        Clear();
                        return CommResult.Failure(CommError.Framing("Frame detector reported an invalid buffer state."));

                    case FrameDetectStatus.FrameReady:
                        if (frameLength <= 0 || frameLength > _length)
                        {
                            Clear();
                            return CommResult.Failure(CommError.Framing("Frame detector returned an invalid frame length."));
                        }

                        if (destination.Length < frameLength)
                        {
                            return CommResult.Failure(
                                CommErrorCode.BufferOverflow,
                                $"Destination span length {destination.Length} is smaller than frame length {frameLength}.");
                        }

                        _buffer.AsSpan(0, frameLength).CopyTo(destination);
                        bytesWritten = frameLength;
                        Discard(frameLength);
                        return CommResult.Success;

                    default:
                        return CommResult.Failure(CommError.Framing($"Unknown frame detect status: {status}."));
                }
            }
        }

        /// <summary>
        /// Extracts the next frame into a newly allocated array when available.
        /// </summary>
        public CommResult<byte[]?> TryReadFrame()
        {
            // Peek detection without consuming to size the array.
            if (_length == 0)
            {
                return CommResult<byte[]?>.Success(null);
            }

            // Use a temporary large enough for capacity; avoid double-buffering complexity.
            var temp = new byte[_length];
            var result = TryReadFrame(temp, out var written);
            if (result.IsFailure)
            {
                return CommResult<byte[]?>.Failure(result.Error!);
            }

            if (written == 0)
            {
                return CommResult<byte[]?>.Success(null);
            }

            if (written == temp.Length)
            {
                return CommResult<byte[]?>.Success(temp);
            }

            var frame = new byte[written];
            Buffer.BlockCopy(temp, 0, frame, 0, written);
            return CommResult<byte[]?>.Success(frame);
        }

        private void Discard(int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (count >= _length)
            {
                _length = 0;
                return;
            }

            Buffer.BlockCopy(_buffer, count, _buffer, 0, _length - count);
            _length -= count;
        }
    }
}
