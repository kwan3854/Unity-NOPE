using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static Result<T, E> OrElse<T, E>(
            this Result<T, E> result,
            Func<Result<T, E>> fallbackFunc)
        {
            return result.IsSuccess ? result : fallbackFunc();
        }

#if NOPE_UNITASK
        // --- UniTask-based Async Methods (OrElse) ---

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static async UniTask<Result<T, E>> OrElse<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<Result<T, E>> fallbackFunc)
        {
            var result = await asyncResult;
            return result.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static UniTask<Result<T, E>> OrElse<T, E>(
            this Result<T, E> result,
            Func<UniTask<Result<T, E>>> asyncFallbackFunc)
        {
            return result.IsSuccess ? UniTask.FromResult(result) : asyncFallbackFunc();
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static async UniTask<Result<T, E>> OrElse<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<UniTask<Result<T, E>>> asyncFallbackFunc)
        {
            var result = await asyncResult;
            return result.IsSuccess ? result : await asyncFallbackFunc();
        }
#endif

#if NOPE_AWAITABLE
        // --- Awaitable Async Methods (OrElse) ---

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<Result<T, E>> fallbackFunc)
        {
            var result = await asyncResult;
            return result.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Result<T, E> result,
            Func<Awaitable<Result<T, E>>> asyncFallbackFunc)
        {
            if (result.IsSuccess) return result;
            return await asyncFallbackFunc();
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise calls the `fallbackFunc` and returns its result.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<Awaitable<Result<T, E>>> asyncFallbackFunc)
        {
            var result = await asyncResult;
            if (result.IsSuccess) return result;
            return await asyncFallbackFunc();
        }
#endif
    }
}