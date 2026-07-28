using System;

namespace IndustrialComm.Checksum
{
    /// <summary>
    /// CRC-16 algorithms commonly used by industrial fieldbuses.
    /// </summary>
    public static class Crc16
    {
        private static readonly ushort[] ModbusTable = CreateTable(0xA001);
        private static readonly ushort[] CcittFalseTable = CreateTableReflected(0x1021);

        /// <summary>
        /// Computes CRC-16/Modbus (poly 0xA001, init 0xFFFF, little-endian wire order).
        /// </summary>
        public static ushort ComputeModbus(ReadOnlySpan<byte> data)
        {
            ushort crc = 0xFFFF;
            for (var i = 0; i < data.Length; i++)
            {
                var index = (byte)(crc ^ data[i]);
                crc = (ushort)((crc >> 8) ^ ModbusTable[index]);
            }

            return crc;
        }

        /// <summary>
        /// Appends CRC-16/Modbus as two little-endian bytes after the payload.
        /// Destination must be at least <paramref name="payload"/>.Length + 2.
        /// </summary>
        public static int AppendModbus(ReadOnlySpan<byte> payload, Span<byte> destination)
        {
            if (destination.Length < payload.Length + 2)
            {
                throw new ArgumentException("Destination buffer is too small.", nameof(destination));
            }

            payload.CopyTo(destination);
            var crc = ComputeModbus(payload);
            destination[payload.Length] = (byte)(crc & 0xFF);
            destination[payload.Length + 1] = (byte)(crc >> 8);
            return payload.Length + 2;
        }

        /// <summary>
        /// Validates a buffer that ends with CRC-16/Modbus (little-endian).
        /// </summary>
        public static bool ValidateModbus(ReadOnlySpan<byte> frameWithCrc)
        {
            if (frameWithCrc.Length < 2)
            {
                return false;
            }

            var payload = frameWithCrc.Slice(0, frameWithCrc.Length - 2);
            var expected = ComputeModbus(payload);
            var actual = (ushort)(frameWithCrc[frameWithCrc.Length - 2] | (frameWithCrc[frameWithCrc.Length - 1] << 8));
            return expected == actual;
        }

        /// <summary>
        /// Computes CRC-16/CCITT-FALSE (poly 0x1021, init 0xFFFF, refin/refout false).
        /// </summary>
        public static ushort ComputeCcittFalse(ReadOnlySpan<byte> data)
        {
            ushort crc = 0xFFFF;
            for (var i = 0; i < data.Length; i++)
            {
                var index = (byte)((crc >> 8) ^ data[i]);
                crc = (ushort)((crc << 8) ^ CcittFalseTable[index]);
            }

            return crc;
        }

        private static ushort[] CreateTable(ushort polynomial)
        {
            var table = new ushort[256];
            for (var i = 0; i < 256; i++)
            {
                ushort value = (ushort)i;
                for (var bit = 0; bit < 8; bit++)
                {
                    var lsb = (value & 1) != 0;
                    value >>= 1;
                    if (lsb)
                    {
                        value ^= polynomial;
                    }
                }

                table[i] = value;
            }

            return table;
        }

        private static ushort[] CreateTableReflected(ushort polynomial)
        {
            // Non-reflected (MSB-first) table for CCITT-FALSE.
            var table = new ushort[256];
            for (var i = 0; i < 256; i++)
            {
                ushort crc = (ushort)(i << 8);
                for (var bit = 0; bit < 8; bit++)
                {
                    if ((crc & 0x8000) != 0)
                    {
                        crc = (ushort)((crc << 1) ^ polynomial);
                    }
                    else
                    {
                        crc <<= 1;
                    }
                }

                table[i] = crc;
            }

            return table;
        }
    }
}
