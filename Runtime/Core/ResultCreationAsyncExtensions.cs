using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// Provides utility methods for creating Result&lt;T&gt; instances
    /// in a more convenient or condition-based manner.
    /// </summary>
    public partial struct Result
    {
#if NOPE_UNITASK
        /// <summary>
        /// Returns Success(successValue) if <paramref name="conditionAsync"/> is true; otherwise Failure(errorMessage).
        /// </summary>
        public static async UniTask<Result<T>> SuccessIf<T>(
            UniTask<bool> conditionAsync,
            T successValue,
            string errorMessage)
        {
            bool cond = await conditionAsync;
            return cond
                ? Result<T>.Success(successValue)
                : Result<T>.Failure(errorMessage);
        }

        /// <summary>
        /// Returns Failure(errorMessage) if <paramref name="conditionAsync"/> is true; otherwise Success(successValue).
        /// </summary>
        public static async UniTask<Result<T>> FailureIf<T>(
            UniTask<bool> conditionAsync,
            T successValue,
            string errorMessage)
        {
            bool cond = await conditionAsync;
            return cond
                ? Result<T>.Failure(errorMessage)
                : Result<T>.Success(successValue);
        }

        /// <summary>
        /// Asynchronously wraps a function that may throw, returning Success or Failure.
        /// </summary>
        public static async UniTask<Result<T>> Of<T>(
            UniTask<T> asyncFunc,
            Func<Exception, string> errorConverter = null)
        {
            try
            {
                T value = await asyncFunc;
                return Result<T>.Success(value);
            }
            catch (Exception ex)
            {
                if (errorConverter != null)
                    return Result<T>.Failure(errorConverter(ex));
                return Result<T>.Failure(ex.Message);
            }
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// Returns Success(successValue) if <paramref name="conditionAwaitable"/> is true; otherwise Failure(errorMessage).
        /// </summary>
        public static async Awaitable<Result<T>> SuccessIf<T>(
            Awaitable<bool> conditionAwaitable,
            T successValue,
            string errorMessage)
        {
            bool cond = await conditionAwaitable;
            return cond
                ? Result<T>.Success(successValue)
                : Result<T>.Failure(errorMessage);
        }

        /// <summary>
        /// Returns Failure(errorMessage) if <paramref name="conditionAwaitable"/> is true; otherwise Success(successValue).
        /// </summary>
        public static async Awaitable<Result<T>> FailureIf<T>(
            Awaitable<bool> conditionAwaitable,
            T successValue,
            string errorMessage)
        {
            bool cond = await conditionAwaitable;
            return cond
                ? Result<T>.Failure(errorMessage)
                : Result<T>.Success(successValue);
        }

        /// <summary>
        /// Asynchronously wraps a function that may throw, returning Success or Failure.
        /// </summary>
        public static async Awaitable<Result<T>> Of<T>(
            Awaitable<T> awaitedFunc,
            Func<Exception, string> errorConverter = null)
        {
            try
            {
                T value = await awaitedFunc;
                return Result<T>.Success(value);
            }
            catch (Exception ex)
            {
                if (errorConverter != null)
                    return Result<T>.Failure(errorConverter(ex));
                return Result<T>.Failure(ex.Message);
            }
        }
#endif // NOPE_AWAITABLE
    }
}