using System;
using IndustrialComm.Binary;

namespace IndustrialComm.Framing
{
    /// <summary>
    /// Detects frames that encode total or payload length in a fixed header field.
    /// </summary>
    public sealed class LengthPrefixFrameDetector : IFrameDetector
    {
        private readonly int _lengthFieldOffset;
        private readonly int _lengthFieldSize;
        private readonly int _lengthAdjustment;
        private readonly int _maxFrameLength;
        private readonly ByteOrder _byteOrder;
        private readonly bool _lengthIncludesHeader;

        /// <summary>
        /// Creates a length-prefix frame detector.
        /// </summary>
        /// <param name="lengthFieldOffset">Offset of the length field from frame start.</param>
        /// <param name="lengthFieldSize">Size of the length field (1, 2, or 4 bytes).</param>
        /// <param name="lengthAdjustment">
        /// Added to the decoded length to obtain the total frame size
        /// (e.g. header bytes not included in the length field).
        /// </param>
        /// <param name="maxFrameLength">Maximum allowed total frame size.</param>
        /// <param name="byteOrder">Byte order of multi-byte length fields.</param>
        /// <param name="lengthIncludesHeader">
        /// When true, the length field already represents the total frame length;
        /// <paramref name="lengthAdjustment"/> is still applied.
        /// </param>
        public LengthPrefixFrameDetector(
            int lengthFieldOffset,
            int lengthFieldSize,
            int lengthAdjustment,
            int maxFrameLength,
            ByteOrder byteOrder = ByteOrder.BigEndian,
            bool lengthIncludesHeader = false)
        {
            if (lengthFieldOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthFieldOffset));
            }

            if (lengthFieldSize != 1 && lengthFieldSize != 2 && lengthFieldSize != 4)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthFieldSize), "Length field size must be 1, 2, or 4.");
            }

            if (maxFrameLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFrameLength));
            }

            _lengthFieldOffset = lengthFieldOffset;
            _lengthFieldSize = lengthFieldSize;
            _lengthAdjustment = lengthAdjustment;
            _maxFrameLength = maxFrameLength;
            _byteOrder = byteOrder;
            _lengthIncludesHeader = lengthIncludesHeader;
        }

        /// <inheritdoc />
        public FrameDetectStatus TryDetect(ReadOnlySpan<byte> buffer, out int frameLength, out int bytesToSkip)
        {
            frameLength = 0;
            bytesToSkip = 0;

            var headerNeeded = _lengthFieldOffset + _lengthFieldSize;
            if (buffer.Length < headerNeeded)
            {
                return FrameDetectStatus.NeedMoreData;
            }

            int declared;
            var field = buffer.Slice(_lengthFieldOffset, _lengthFieldSize);
            declared = _lengthFieldSize switch
            {
                1 => field[0],
                2 => BinaryCodec.ReadUInt16(field, _byteOrder),
                _ => unchecked((int)BinaryCodec.ReadUInt32(field, _byteOrder)),
            };

            if (declared < 0)
            {
                return FrameDetectStatus.Invalid;
            }

            int total;
            if (_lengthIncludesHeader)
            {
                total = declared + _lengthAdjustment;
            }
            else
            {
                total = declared + _lengthAdjustment;
            }

            if (total <= 0 || total > _maxFrameLength)
            {
                return FrameDetectStatus.Invalid;
            }

            if (buffer.Length < total)
            {
                return FrameDetectStatus.NeedMoreData;
            }

            frameLength = total;
            return FrameDetectStatus.FrameReady;
        }
    }
}
