namespace IndustrialComm.Binary
{
    /// <summary>
    /// Byte order used when encoding multi-byte industrial data types.
    /// </summary>
    public enum ByteOrder
    {
        /// <summary>Big-endian (network / most PLCs for words).</summary>
        BigEndian = 0,

        /// <summary>Little-endian.</summary>
        LittleEndian = 1,
    }
}
