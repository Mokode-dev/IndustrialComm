using System;

namespace IndustrialComm.Framing
{
    /// <summary>
    /// Detects frames terminated by a fixed delimiter sequence (e.g. CR/LF).
    /// Optionally requires a start delimiter.
    /// </summary>
    public sealed class DelimiterFrameDetector : IFrameDetector
    {
        private readonly byte[] _start;
        private readonly byte[] _end;
        private readonly int _maxFrameLength;
        private readonly bool _includeDelimiter;

        /// <summary>
        /// Creates a delimiter-based frame detector.
        /// </summary>
        /// <param name="endDelimiter">Required end delimiter sequence.</param>
        /// <param name="maxFrameLength">Maximum frame size including delimiters.</param>
        /// <param name="startDelimiter">Optional start delimiter; when set, leading garbage is skipped.</param>
        /// <param name="includeDelimiter">When true, frame length includes the end delimiter.</param>
        public DelimiterFrameDetector(
            ReadOnlySpan<byte> endDelimiter,
            int maxFrameLength,
            ReadOnlySpan<byte> startDelimiter = default,
            bool includeDelimiter = true)
        {
            if (endDelimiter.IsEmpty)
            {
                throw new ArgumentException("End delimiter is required.", nameof(endDelimiter));
            }

            if (maxFrameLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFrameLength));
            }

            _end = endDelimiter.ToArray();
            _start = startDelimiter.IsEmpty ? Array.Empty<byte>() : startDelimiter.ToArray();
            _maxFrameLength = maxFrameLength;
            _includeDelimiter = includeDelimiter;
        }

        /// <inheritdoc />
        public FrameDetectStatus TryDetect(ReadOnlySpan<byte> buffer, out int frameLength, out int bytesToSkip)
        {
            frameLength = 0;
            bytesToSkip = 0;

            var searchStart = 0;
            if (_start.Length > 0)
            {
                var startIndex = IndexOf(buffer, _start);
                if (startIndex < 0)
                {
                    // Keep a small tail that might be a partial start delimiter.
                    if (buffer.Length >= _start.Length)
                    {
                        bytesToSkip = buffer.Length - _start.Length + 1;
                        return bytesToSkip > 0 ? FrameDetectStatus.Skip : FrameDetectStatus.NeedMoreData;
                    }

                    return FrameDetectStatus.NeedMoreData;
                }

                if (startIndex > 0)
                {
                    bytesToSkip = startIndex;
                    return FrameDetectStatus.Skip;
                }

                searchStart = _start.Length;
            }

            var endIndex = IndexOf(buffer.Slice(searchStart), _end);
            if (endIndex < 0)
            {
                if (buffer.Length >= _maxFrameLength)
                {
                    return FrameDetectStatus.Invalid;
                }

                return FrameDetectStatus.NeedMoreData;
            }

            var absoluteEnd = searchStart + endIndex;
            var total = _includeDelimiter ? absoluteEnd + _end.Length : absoluteEnd;
            if (total <= 0 || total > _maxFrameLength)
            {
                return FrameDetectStatus.Invalid;
            }

            frameLength = total;
            return FrameDetectStatus.FrameReady;
        }

        private static int IndexOf(ReadOnlySpan<byte> haystack, ReadOnlySpan<byte> needle)
        {
            if (needle.Length == 0)
            {
                return 0;
            }

            if (haystack.Length < needle.Length)
            {
                return -1;
            }

            for (var i = 0; i <= haystack.Length - needle.Length; i++)
            {
                if (haystack.Slice(i, needle.Length).SequenceEqual(needle))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
