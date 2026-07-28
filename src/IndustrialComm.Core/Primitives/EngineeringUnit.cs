using System;

namespace IndustrialComm.Primitives
{
    /// <summary>
    /// Lightweight engineering unit label for process values (e.g. "°C", "kPa", "rpm").
    /// </summary>
    public readonly struct EngineeringUnit : IEquatable<EngineeringUnit>
    {
        /// <summary>
        /// Creates a unit from a symbol string.
        /// </summary>
        public EngineeringUnit(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ArgumentException("Unit symbol is required.", nameof(symbol));
            }

            Symbol = symbol;
        }

        /// <summary>Unit symbol or short name.</summary>
        public string Symbol { get; }

        /// <summary>Dimensionless / none.</summary>
        public static EngineeringUnit None { get; } = new EngineeringUnit("1");

        /// <summary>Degrees Celsius.</summary>
        public static EngineeringUnit Celsius { get; } = new EngineeringUnit("°C");

        /// <summary>Percent.</summary>
        public static EngineeringUnit Percent { get; } = new EngineeringUnit("%");

        /// <inheritdoc />
        public bool Equals(EngineeringUnit other)
            => string.Equals(Symbol, other.Symbol, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is EngineeringUnit other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Symbol);

        /// <inheritdoc />
        public override string ToString() => Symbol;

        /// <summary>Equality operator.</summary>
        public static bool operator ==(EngineeringUnit left, EngineeringUnit right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(EngineeringUnit left, EngineeringUnit right) => !left.Equals(right);
    }
}
