using System;

namespace IndustrialComm.Primitives
{
    /// <summary>
    /// UTC timestamp for industrial values, stored as Unix milliseconds for portability.
    /// </summary>
    public readonly struct CommTimestamp : IEquatable<CommTimestamp>, IComparable<CommTimestamp>
    {
        /// <summary>
        /// Creates a timestamp from Unix epoch milliseconds (UTC).
        /// </summary>
        public CommTimestamp(long unixTimeMilliseconds)
        {
            UnixTimeMilliseconds = unixTimeMilliseconds;
        }

        /// <summary>Milliseconds since Unix epoch (UTC).</summary>
        public long UnixTimeMilliseconds { get; }

        /// <summary>Current UTC time.</summary>
        public static CommTimestamp UtcNow =>
            new CommTimestamp(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        /// <summary>Creates from a <see cref="DateTimeOffset"/>.</summary>
        public static CommTimestamp FromDateTimeOffset(DateTimeOffset value)
            => new CommTimestamp(value.ToUnixTimeMilliseconds());

        /// <summary>Converts to <see cref="DateTimeOffset"/> (UTC).</summary>
        public DateTimeOffset ToDateTimeOffset()
            => DateTimeOffset.FromUnixTimeMilliseconds(UnixTimeMilliseconds);

        /// <inheritdoc />
        public int CompareTo(CommTimestamp other)
            => UnixTimeMilliseconds.CompareTo(other.UnixTimeMilliseconds);

        /// <inheritdoc />
        public bool Equals(CommTimestamp other)
            => UnixTimeMilliseconds == other.UnixTimeMilliseconds;

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is CommTimestamp other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => UnixTimeMilliseconds.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => ToDateTimeOffset().ToString("O");

        /// <summary>Equality operator.</summary>
        public static bool operator ==(CommTimestamp left, CommTimestamp right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(CommTimestamp left, CommTimestamp right) => !left.Equals(right);

        /// <summary>Less-than operator.</summary>
        public static bool operator <(CommTimestamp left, CommTimestamp right) => left.CompareTo(right) < 0;

        /// <summary>Greater-than operator.</summary>
        public static bool operator >(CommTimestamp left, CommTimestamp right) => left.CompareTo(right) > 0;

        /// <summary>Less-than-or-equal operator.</summary>
        public static bool operator <=(CommTimestamp left, CommTimestamp right) => left.CompareTo(right) <= 0;

        /// <summary>Greater-than-or-equal operator.</summary>
        public static bool operator >=(CommTimestamp left, CommTimestamp right) => left.CompareTo(right) >= 0;
    }
}
