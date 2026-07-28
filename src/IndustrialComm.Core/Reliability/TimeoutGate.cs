using System;
using System.Threading;
using System.Threading.Tasks;
using IndustrialComm.Results;

namespace IndustrialComm.Reliability
{
    /// <summary>
    /// Runs asynchronous operations with a timeout that cooperates with <see cref="CancellationToken"/>.
    /// </summary>
    public static class TimeoutGate
    {
        /// <summary>
        /// Executes <paramref name="operation"/> and fails with <see cref="CommErrorCode.Timeout"/>
        /// when <paramref name="timeout"/> elapses before completion.
        /// </summary>
        public static async ValueTask<CommResult> ExecuteAsync(
            Func<CancellationToken, ValueTask<CommResult>> operation,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (timeout <= TimeSpan.Zero)
            {
                return CommResult.Failure(CommError.InvalidArgument("Timeout must be greater than zero."));
            }

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            try
            {
                return await operation(timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException ex)
            {
                return CommResult.Failure(CommError.Timeout(exception: ex));
            }
            catch (Exception ex)
            {
                return CommResult.Failure(CommError.FromException(ex));
            }
        }

        /// <summary>
        /// Executes a value-producing operation with a timeout.
        /// </summary>
        public static async ValueTask<CommResult<T>> ExecuteAsync<T>(
            Func<CancellationToken, ValueTask<CommResult<T>>> operation,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (timeout <= TimeSpan.Zero)
            {
                return CommResult<T>.Failure(CommError.InvalidArgument("Timeout must be greater than zero."));
            }

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            try
            {
                return await operation(timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException ex)
            {
                return CommResult<T>.Failure(CommError.Timeout(exception: ex));
            }
            catch (Exception ex)
            {
                return CommResult<T>.Failure(CommError.FromException(ex));
            }
        }

        /// <summary>
        /// Waits for a task with a timeout, mapping timeout to <see cref="CommResult{T}"/>.
        /// </summary>
        public static async ValueTask<CommResult<T>> WaitAsync<T>(
            Task<T> task,
            TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (timeout <= TimeSpan.Zero)
            {
                return CommResult<T>.Failure(CommError.InvalidArgument("Timeout must be greater than zero."));
            }

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(timeout);

            var completed = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, timeoutCts.Token)).ConfigureAwait(false);
            if (completed == task)
            {
                try
                {
                    var value = await task.ConfigureAwait(false);
                    return CommResult<T>.Success(value);
                }
                catch (Exception ex)
                {
                    return CommResult<T>.Failure(CommError.FromException(ex));
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            return CommResult<T>.Failure(CommError.Timeout());
        }
    }
}
