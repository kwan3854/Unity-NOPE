using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {
        /// <summary>
        /// Returns Success(successValue) if condition is true; otherwise Failure(error).
        /// </summary>
        public static Result<T, E> SuccessIf<T, E>(
            bool condition,
            T successValue,
            E error)
        {
            return condition
                ? Result<T, E>.Success(successValue)
                : Result<T, E>.Failure(error);
        }

        /// <summary>
        /// Returns Success(successValue) if condition() is true; otherwise Failure(error).
        /// Lazy-evaluated predicate.
        /// </summary>
        public static Result<T, E> SuccessIf<T, E>(
            Func<bool> condition,
            T successValue,
            E error)
        {
            return condition()
                ? Result<T, E>.Success(successValue)
                : Result<T, E>.Failure(error);
        }

        /// <summary>
        /// Returns Failure(error) if condition is true; otherwise Success(successValue).
        /// </summary>
        public static Result<T, E> FailureIf<T, E>(
            bool condition,
            T successValue,
            E error)
        {
            return condition
                ? Result<T, E>.Failure(error)
                : Result<T, E>.Success(successValue);
        }

        /// <summary>
        /// Returns Failure(error) if condition() is true; otherwise Success(successValue).
        /// </summary>
        public static Result<T, E> FailureIf<T, E>(
            Func<bool> condition,
            T successValue,
            E error)
        {
            return condition()
                ? Result<T, E>.Failure(error)
                : Result<T, E>.Success(successValue);
        }
        
        /// <summary>
        /// Wraps a function call that may throw and returns:
        /// - Success(result) if no exception
        /// - Failure(errorConverter(ex)) if an exception is thrown
        /// </summary>
        public static Result<T, E> Of<T, E>(
            Func<T> func,
            Func<Exception, E> errorConverter)
        {
            try
            {
                var value = func();
                return value; // implicit => Result<T,E>.Success(value)
            }
            catch (Exception ex)
            {
                return errorConverter(ex);
            }
        }

#if NOPE_UNITASK
        /// <summary>
        /// Returns Success(successValue) if conditionAsync is true; otherwise Failure(error).
        /// </summary>
        public static async UniTask<Result<T, E>> SuccessIf<T, E>(
            Func<UniTask<bool>> conditionAsync,
            T successValue,
            E error)
        {
            bool cond = await conditionAsync();
            return cond
                ? Result<T, E>.Success(successValue)
                : Result<T, E>.Failure(error);
        }

        /// <summary>
        /// Returns Failure(error) if conditionAsync is true; otherwise Success(successValue).
        /// </summary>
        public static async UniTask<Result<T, E>> FailureIf<T, E>(
            Func<UniTask<bool>> conditionAsync,
            T successValue,
            E error)
        {
            bool cond = await conditionAsync();
            return cond
                ? Result<T, E>.Failure(error)
                : Result<T, E>.Success(successValue);
        }
        
        /// <summary>
        /// Asynchronously wraps a function that may throw, returning Success or Failure.
        /// </summary>
        public static async UniTask<Result<T, E>> Of<T, E>(
            Func<UniTask<T>> asyncFunc,
            Func<Exception, E> errorConverter)
        {
            try
            {
                T value = await asyncFunc();
                return value;
            }
            catch (Exception ex)
            {
                return errorConverter(ex);
            }
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Returns Success(successValue) if conditionAwaitable is true; otherwise Failure(error).
        /// </summary>
        public static async Awaitable<Result<T, E>> SuccessIf<T, E>(
            Func<Awaitable<bool>> conditionAwaitable,
            T successValue,
            E error)
        {
            bool cond = await conditionAwaitable();
            return cond
                ? Result<T, E>.Success(successValue)
                : Result<T, E>.Failure(error);
        }

        /// <summary>
        /// Returns Failure(error) if conditionAwaitable is true; otherwise Success(successValue).
        /// </summary>
        public static async Awaitable<Result<T, E>> FailureIf<T, E>(
            Func<Awaitable<bool>> conditionAwaitable,
            T successValue,
            E error)
        {
            bool cond = await conditionAwaitable();
            return cond
                ? Result<T, E>.Failure(error)
                : Result<T, E>.Success(successValue);
        }

        /// <summary>
        /// Asynchronously wraps a function that may throw, returning Success or Failure.
        /// </summary>
        public static async Awaitable<Result<T, E>> Of<T, E>(
            Func<Awaitable<T>> awaitedFunc,
            Func<Exception, E> errorConverter)
        {
            try
            {
                T value = await awaitedFunc();
                return value;
            }
            catch (Exception ex)
            {
                return errorConverter(ex);
            }
        }
#endif

    }
}
