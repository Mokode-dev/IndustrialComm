using System;
using System.Threading;
using System.Threading.Tasks;
using IndustrialComm.Results;

namespace IndustrialComm.Reliability
{
    /// <summary>
    /// Options for <see cref="RetryPolicy"/>.
    /// </summary>
    public sealed class RetryOptions
    {
        /// <summary>Maximum number of attempts including the first try. Default: 3.</summary>
        public int MaxAttempts { get; set; } = 3;

        /// <summary>Base delay before the first retry. Default: 100ms.</summary>
        public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>Maximum delay between retries. Default: 5s.</summary>
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>Exponential backoff multiplier. Default: 2.0.</summary>
        public double BackoffFactor { get; set; } = 2.0;

        /// <summary>When true, adds full-jitter to the delay. Default: true.</summary>
        public bool UseJitter { get; set; } = true;

        /// <summary>
        /// Predicate deciding whether a failure should be retried.
        /// Default retries timeouts, transient I/O, and checksum mismatches.
        /// </summary>
        public Func<CommError, bool>? ShouldRetry { get; set; }
    }

    /// <summary>
    /// Executes asynchronous operations with configurable retry and exponential backoff.
    /// Cancellation is never swallowed.
    /// </summary>
    public sealed class RetryPolicy
    {
        private readonly RetryOptions _options;
        private readonly Random _random = new Random();

        /// <summary>Creates a retry policy with the given options.</summary>
        public RetryPolicy(RetryOptions? options = null)
        {
            _options = options ?? new RetryOptions();
            if (_options.MaxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(options), "MaxAttempts must be at least 1.");
            }
        }

        /// <summary>
        /// Executes <paramref name="operation"/> until success, non-retryable failure, or attempts exhausted.
        /// </summary>
        public async ValueTask<CommResult> ExecuteAsync(
            Func<CancellationToken, ValueTask<CommResult>> operation,
            CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            CommError? lastError = null;
            for (var attempt = 1; attempt <= _options.MaxAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CommResult result;
                try
                {
                    result = await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    result = CommResult.Failure(CommError.FromException(ex));
                }

                if (result.IsSuccess)
                {
                    return result;
                }

                lastError = result.Error!;
                if (attempt >= _options.MaxAttempts || !IsRetryable(lastError))
                {
                    return result;
                }

                await DelayAsync(attempt, cancellationToken).ConfigureAwait(false);
            }

            return CommResult.Failure(
                lastError ?? new CommError(CommErrorCode.RetryExhausted, "Retry attempts were exhausted."));
        }

        /// <summary>
        /// Executes a value-producing operation with retry.
        /// </summary>
        public async ValueTask<CommResult<T>> ExecuteAsync<T>(
            Func<CancellationToken, ValueTask<CommResult<T>>> operation,
            CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            CommError? lastError = null;
            for (var attempt = 1; attempt <= _options.MaxAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                CommResult<T> result;
                try
                {
                    result = await operation(cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    result = CommResult<T>.Failure(CommError.FromException(ex));
                }

                if (result.IsSuccess)
                {
                    return result;
                }

                lastError = result.Error!;
                if (attempt >= _options.MaxAttempts || !IsRetryable(lastError))
                {
                    return result;
                }

                await DelayAsync(attempt, cancellationToken).ConfigureAwait(false);
            }

            return CommResult<T>.Failure(
                lastError ?? new CommError(CommErrorCode.RetryExhausted, "Retry attempts were exhausted."));
        }

        private bool IsRetryable(CommError error)
        {
            if (_options.ShouldRetry is not null)
            {
                return _options.ShouldRetry(error);
            }

            return error.Code == CommErrorCode.Timeout
                || error.Code == CommErrorCode.ChecksumMismatch
                || error.Code == CommErrorCode.ReadFailed
                || error.Code == CommErrorCode.WriteFailed
                || error.Code == CommErrorCode.FramingError
                || error.Code == CommErrorCode.ConnectionClosed;
        }

        private async Task DelayAsync(int attempt, CancellationToken cancellationToken)
        {
            var exp = Math.Pow(_options.BackoffFactor, attempt - 1);
            var delayMs = _options.BaseDelay.TotalMilliseconds * exp;
            delayMs = Math.Min(delayMs, _options.MaxDelay.TotalMilliseconds);

            if (_options.UseJitter && delayMs > 0)
            {
                lock (_random)
                {
                    delayMs *= _random.NextDouble();
                }
            }

            if (delayMs > 0)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(delayMs), cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
