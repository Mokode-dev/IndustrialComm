using System;
using System.Diagnostics;

namespace IndustrialComm.Results
{
    /// <summary>
    /// Result of an industrial communication operation without a payload.
    /// Expected failures are represented as <see cref="Error"/> rather than thrown exceptions.
    /// </summary>
    [DebuggerDisplay("{IsSuccess ? \"Success\" : Error}")]
    public readonly struct CommResult : IEquatable<CommResult>
    {
        private readonly CommError? _error;

        private CommResult(CommError? error)
        {
            _error = error;
        }

        /// <summary>True when the operation succeeded.</summary>
        public bool IsSuccess => _error is null;

        /// <summary>True when the operation failed.</summary>
        public bool IsFailure => _error is not null;

        /// <summary>Error details when <see cref="IsFailure"/>; otherwise null.</summary>
        public CommError? Error => _error;

        /// <summary>Successful result.</summary>
        public static CommResult Success { get; } = new CommResult(null);

        /// <summary>Creates a failed result.</summary>
        public static CommResult Failure(CommError error)
        {
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return new CommResult(error);
        }

        /// <summary>Creates a failed result from code and message.</summary>
        public static CommResult Failure(CommErrorCode code, string message, Exception? exception = null, string? detail = null)
            => Failure(new CommError(code, message, exception, detail));

        /// <summary>Throws <see cref="InvalidOperationException"/> when the result is a failure.</summary>
        public void EnsureSuccess()
        {
            if (_error is not null)
            {
                throw new InvalidOperationException(_error.ToString(), _error.Exception);
            }
        }

        /// <inheritdoc />
        public bool Equals(CommResult other) => Equals(_error, other._error);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CommResult other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _error is null ? 0 : _error.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => IsSuccess ? "Success" : _error!.ToString();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(CommResult left, CommResult right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(CommResult left, CommResult right) => !left.Equals(right);
    }

    /// <summary>
    /// Result of an industrial communication operation with a payload of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Payload type.</typeparam>
    [DebuggerDisplay("{IsSuccess ? Value : Error}")]
    public readonly struct CommResult<T> : IEquatable<CommResult<T>>
    {
        private readonly T? _value;
        private readonly CommError? _error;

        private CommResult(T? value, CommError? error)
        {
            _value = value;
            _error = error;
        }

        /// <summary>True when the operation succeeded.</summary>
        public bool IsSuccess => _error is null;

        /// <summary>True when the operation failed.</summary>
        public bool IsFailure => _error is not null;

        /// <summary>Payload when successful; otherwise default.</summary>
        public T? Value => _value;

        /// <summary>Error details when <see cref="IsFailure"/>; otherwise null.</summary>
        public CommError? Error => _error;

        /// <summary>Creates a successful result.</summary>
        public static CommResult<T> Success(T value) => new CommResult<T>(value, null);

        /// <summary>Creates a failed result.</summary>
        public static CommResult<T> Failure(CommError error)
        {
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return new CommResult<T>(default, error);
        }

        /// <summary>Creates a failed result from code and message.</summary>
        public static CommResult<T> Failure(CommErrorCode code, string message, Exception? exception = null, string? detail = null)
            => Failure(new CommError(code, message, exception, detail));

        /// <summary>
        /// Returns the payload or throws when failed.
        /// </summary>
        public T GetValueOrThrow()
        {
            if (_error is not null)
            {
                throw new InvalidOperationException(_error.ToString(), _error.Exception);
            }

            return _value!;
        }

        /// <summary>Maps a successful payload; failures pass through.</summary>
        public CommResult<TOut> Map<TOut>(Func<T, TOut> mapper)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            return IsSuccess
                ? CommResult<TOut>.Success(mapper(_value!))
                : CommResult<TOut>.Failure(_error!);
        }

        /// <summary>Implicit conversion from a non-generic failure.</summary>
        public static implicit operator CommResult<T>(CommResult result)
        {
            return result.IsSuccess
                ? throw new InvalidCastException("Cannot convert a non-generic success to CommResult<T> without a value.")
                : Failure(result.Error!);
        }

        /// <inheritdoc />
        public bool Equals(CommResult<T> other)
            => Equals(_error, other._error) && Equals(_value, other._value);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CommResult<T> other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = _error is null ? 0 : _error.GetHashCode();
                hash = (hash * 397) ^ (_value is null ? 0 : _value.GetHashCode());
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString() => IsSuccess ? (_value?.ToString() ?? "Success") : _error!.ToString();

        /// <summary>Equality operator.</summary>
        public static bool operator ==(CommResult<T> left, CommResult<T> right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(CommResult<T> left, CommResult<T> right) => !left.Equals(right);
    }
}
