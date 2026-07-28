using System;

namespace IndustrialComm.Binary
{
    /// <summary>
    /// Bit and coil packing helpers commonly used by discrete industrial protocols.
    /// </summary>
    public static class BitPacker
    {
        /// <summary>
        /// Packs boolean coils into bytes (LSB of each byte is the first coil), Modbus-style.
        /// </summary>
        /// <param name="coils">Coil values.</param>
        /// <param name="destination">Destination buffer; must be large enough.</param>
        /// <returns>Number of bytes written.</returns>
        public static int PackCoils(ReadOnlySpan<bool> coils, Span<byte> destination)
        {
            var byteCount = (coils.Length + 7) / 8;
            if (destination.Length < byteCount)
            {
                throw new ArgumentException("Destination buffer is too small.", nameof(destination));
            }

            destination.Slice(0, byteCount).Clear();
            for (var i = 0; i < coils.Length; i++)
            {
                if (coils[i])
                {
                    destination[i / 8] |= (byte)(1 << (i % 8));
                }
            }

            return byteCount;
        }

        /// <summary>
        /// Unpacks Modbus-style coil bytes into booleans.
        /// </summary>
        public static void UnpackCoils(ReadOnlySpan<byte> source, Span<bool> destination)
        {
            for (var i = 0; i < destination.Length; i++)
            {
                var b = source[i / 8];
                destination[i] = (b & (1 << (i % 8))) != 0;
            }
        }

        /// <summary>Gets a single bit from a byte buffer (bit 0 = LSB of first byte).</summary>
        public static bool GetBit(ReadOnlySpan<byte> source, int bitIndex)
        {
            if (bitIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex));
            }

            var byteIndex = bitIndex / 8;
            if (byteIndex >= source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex));
            }

            return (source[byteIndex] & (1 << (bitIndex % 8))) != 0;
        }

        /// <summary>Sets a single bit in a byte buffer (bit 0 = LSB of first byte).</summary>
        public static void SetBit(Span<byte> destination, int bitIndex, bool value)
        {
            if (bitIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex));
            }

            var byteIndex = bitIndex / 8;
            if (byteIndex >= destination.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bitIndex));
            }

            if (value)
            {
                destination[byteIndex] |= (byte)(1 << (bitIndex % 8));
            }
            else
            {
                destination[byteIndex] &= (byte)~(1 << (bitIndex % 8));
            }
        }
    }
}
