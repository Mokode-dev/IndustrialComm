using System;

namespace IndustrialComm.Checksum
{
    /// <summary>
    /// CRC-32 (ISO-HDLC / Ethernet / PKZIP style): poly 0xEDB88320, init 0xFFFFFFFF, xorout 0xFFFFFFFF.
    /// </summary>
    public static class Crc32
    {
        private static readonly uint[] Table = CreateTable();

        /// <summary>Computes CRC-32 over the specified data.</summary>
        public static uint Compute(ReadOnlySpan<byte> data)
        {
            uint crc = 0xFFFFFFFF;
            for (var i = 0; i < data.Length; i++)
            {
                var index = (byte)((crc ^ data[i]) & 0xFF);
                crc = (crc >> 8) ^ Table[index];
            }

            return crc ^ 0xFFFFFFFF;
        }

        private static uint[] CreateTable()
        {
            const uint polynomial = 0xEDB88320;
            var table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                var value = i;
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
    }
}
