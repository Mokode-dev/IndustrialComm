using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace IndustrialComm.Binary
{
    /// <summary>
    /// Allocation-friendly binary encode/decode helpers for industrial protocols.
    /// Hot paths use <see cref="Span{T}"/> / <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    public static class BinaryCodec
    {
        /// <summary>Reads a <see cref="ushort"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadUInt16BigEndian(source)
                : BinaryPrimitives.ReadUInt16LittleEndian(source);

        /// <summary>Writes a <see cref="ushort"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16(Span<byte> destination, ushort value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteUInt16BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt16LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a <see cref="short"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadInt16BigEndian(source)
                : BinaryPrimitives.ReadInt16LittleEndian(source);

        /// <summary>Writes a <see cref="short"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16(Span<byte> destination, short value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteInt16BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteInt16LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a <see cref="uint"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadUInt32BigEndian(source)
                : BinaryPrimitives.ReadUInt32LittleEndian(source);

        /// <summary>Writes a <see cref="uint"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32(Span<byte> destination, uint value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteUInt32BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt32LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a <see cref="int"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadInt32BigEndian(source)
                : BinaryPrimitives.ReadInt32LittleEndian(source);

        /// <summary>Writes a <see cref="int"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32(Span<byte> destination, int value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteInt32BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a <see cref="ulong"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadUInt64BigEndian(source)
                : BinaryPrimitives.ReadUInt64LittleEndian(source);

        /// <summary>Writes a <see cref="ulong"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64(Span<byte> destination, ulong value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteUInt64BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt64LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a <see cref="long"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64(ReadOnlySpan<byte> source, ByteOrder order)
            => order == ByteOrder.BigEndian
                ? BinaryPrimitives.ReadInt64BigEndian(source)
                : BinaryPrimitives.ReadInt64LittleEndian(source);

        /// <summary>Writes a <see cref="long"/> with the specified byte order.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64(Span<byte> destination, long value, ByteOrder order)
        {
            if (order == ByteOrder.BigEndian)
            {
                BinaryPrimitives.WriteInt64BigEndian(destination, value);
            }
            else
            {
                BinaryPrimitives.WriteInt64LittleEndian(destination, value);
            }
        }

        /// <summary>Reads a single-precision float with the specified byte order.</summary>
        public static float ReadSingle(ReadOnlySpan<byte> source, ByteOrder order)
        {
            var bits = ReadUInt32(source, order);
#if NETSTANDARD2_0
            return Int32BitsToSingle(unchecked((int)bits));
#else
            return BitConverter.Int32BitsToSingle(unchecked((int)bits));
#endif
        }

        /// <summary>Writes a single-precision float with the specified byte order.</summary>
        public static void WriteSingle(Span<byte> destination, float value, ByteOrder order)
        {
#if NETSTANDARD2_0
            var bits = unchecked((uint)SingleToInt32Bits(value));
#else
            var bits = unchecked((uint)BitConverter.SingleToInt32Bits(value));
#endif
            WriteUInt32(destination, bits, order);
        }

        /// <summary>Reads a double-precision float with the specified byte order.</summary>
        public static double ReadDouble(ReadOnlySpan<byte> source, ByteOrder order)
        {
            var bits = ReadUInt64(source, order);
#if NETSTANDARD2_0
            return BitConverter.Int64BitsToDouble(unchecked((long)bits));
#else
            return BitConverter.Int64BitsToDouble(unchecked((long)bits));
#endif
        }

        /// <summary>Writes a double-precision float with the specified byte order.</summary>
        public static void WriteDouble(Span<byte> destination, double value, ByteOrder order)
        {
            var bits = unchecked((ulong)BitConverter.DoubleToInt64Bits(value));
            WriteUInt64(destination, bits, order);
        }

        /// <summary>
        /// Reads a fixed-length ASCII string, trimming trailing NULs and spaces.
        /// </summary>
        public static string ReadAscii(ReadOnlySpan<byte> source)
        {
            var end = source.Length;
            while (end > 0 && (source[end - 1] == 0 || source[end - 1] == (byte)' '))
            {
                end--;
            }

            if (end == 0)
            {
                return string.Empty;
            }

#if NETSTANDARD2_0
            return Encoding.ASCII.GetString(source.Slice(0, end).ToArray());
#else
            return Encoding.ASCII.GetString(source.Slice(0, end));
#endif
        }

        /// <summary>
        /// Writes an ASCII string into a fixed-length buffer, padding with NULs.
        /// </summary>
        public static void WriteAscii(Span<byte> destination, string value, byte pad = 0)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var count = Math.Min(value.Length, destination.Length);
#if NETSTANDARD2_0
            var bytes = Encoding.ASCII.GetBytes(value);
            bytes.AsSpan(0, count).CopyTo(destination);
#else
            Encoding.ASCII.GetBytes(value.AsSpan(0, count), destination);
#endif
            if (count < destination.Length)
            {
                destination.Slice(count).Fill(pad);
            }
        }

        /// <summary>
        /// Swaps adjacent bytes inside a 16-bit word (AB → BA). Useful for mixed PLC word layouts.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SwapBytes(ushort value)
            => (ushort)((value >> 8) | (value << 8));

        /// <summary>
        /// Swaps 16-bit words inside a 32-bit dword (ABCD → CDAB).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SwapWords(uint value)
            => (value >> 16) | (value << 16);

        /// <summary>
        /// Fully reverses byte order of a 32-bit value (ABCD → DCBA).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReverseBytes(uint value)
            => BinaryPrimitives.ReverseEndianness(value);

#if NETSTANDARD2_0
        private static float Int32BitsToSingle(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToSingle(bytes, 0);
        }

        private static int SingleToInt32Bits(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            return BitConverter.ToInt32(bytes, 0);
        }
#endif
    }
}
