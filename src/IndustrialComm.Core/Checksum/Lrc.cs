using System;

namespace IndustrialComm.Checksum
{
    /// <summary>
    /// Longitudinal Redundancy Check (LRC) used by Modbus ASCII and similar protocols.
    /// </summary>
    public static class Lrc
    {
        /// <summary>
        /// Computes the two's-complement LRC of the data bytes (Modbus ASCII style).
        /// </summary>
        public static byte Compute(ReadOnlySpan<byte> data)
        {
            byte sum = 0;
            for (var i = 0; i < data.Length; i++)
            {
                sum += data[i];
            }

            return (byte)((0 - sum) & 0xFF);
        }

        /// <summary>
        /// Validates that the last byte of the buffer is the LRC of the preceding bytes.
        /// </summary>
        public static bool Validate(ReadOnlySpan<byte> frameWithLrc)
        {
            if (frameWithLrc.Length < 1)
            {
                return false;
            }

            var payload = frameWithLrc.Slice(0, frameWithLrc.Length - 1);
            return Compute(payload) == frameWithLrc[frameWithLrc.Length - 1];
        }
    }
}
