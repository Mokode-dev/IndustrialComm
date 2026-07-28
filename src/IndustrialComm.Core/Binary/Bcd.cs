using System;

namespace IndustrialComm.Binary
{
    /// <summary>
    /// Binary-coded decimal (BCD) conversion helpers used by many PLCs and instruments.
    /// </summary>
    public static class Bcd
    {
        /// <summary>Decodes a single BCD byte (0x00–0x99) to an integer 0–99.</summary>
        public static int DecodeByte(byte value)
        {
            var hi = value >> 4;
            var lo = value & 0x0F;
            if (hi > 9 || lo > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid BCD nibble.");
            }

            return (hi * 10) + lo;
        }

        /// <summary>Encodes an integer 0–99 into a BCD byte.</summary>
        public static byte EncodeByte(int value)
        {
            if (value < 0 || value > 99)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be between 0 and 99.");
            }

            return (byte)(((value / 10) << 4) | (value % 10));
        }

        /// <summary>
        /// Decodes a big-endian BCD buffer into an unsigned integer.
        /// Each byte contributes two decimal digits.
        /// </summary>
        public static ulong Decode(ReadOnlySpan<byte> source)
        {
            ulong result = 0;
            for (var i = 0; i < source.Length; i++)
            {
                result = (result * 100UL) + (ulong)DecodeByte(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Encodes an unsigned integer into a fixed-length big-endian BCD buffer.
        /// </summary>
        public static void Encode(ulong value, Span<byte> destination)
        {
            for (var i = destination.Length - 1; i >= 0; i--)
            {
                destination[i] = EncodeByte((int)(value % 100UL));
                value /= 100UL;
            }

            if (value != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value does not fit in the destination BCD buffer.");
            }
        }
    }
}
