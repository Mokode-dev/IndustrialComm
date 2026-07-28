namespace IndustrialComm.Primitives
{
    /// <summary>
    /// Data quality classification aligned with common SCADA / OPC semantics.
    /// </summary>
    public enum DataQuality : byte
    {
        /// <summary>Quality is unknown.</summary>
        Unknown = 0,

        /// <summary>Value is good and usable.</summary>
        Good = 1,

        /// <summary>Value is uncertain (e.g. last known, sensor degraded).</summary>
        Uncertain = 2,

        /// <summary>Value is bad and should not be used for control.</summary>
        Bad = 3,
    }
}
