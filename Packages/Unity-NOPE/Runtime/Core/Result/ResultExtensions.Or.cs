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
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static Result<T, E> Or<T, E>(
            this Result<T, E> result,
            Result<T, E> fallbackResult)
        {
            return result.IsSuccess ? result : fallbackResult;
        }

#if NOPE_UNITASK
        // --- UniTask --
        
        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async UniTask<Result<T, E>> Or<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Result<T, E> fallbackResult)
        {
            var result = await asyncResult;
            return result.Or(fallbackResult);
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async UniTask<Result<T, E>> Or<T, E>(
            this Result<T, E> result,
            UniTask<Result<T, E>> asyncFallbackResult)
        {
            return result.IsSuccess ? result : await asyncFallbackResult;
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async UniTask<Result<T, E>> Or<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            UniTask<Result<T, E>> asyncFallbackResult)
        {
            var result = await asyncResult;
            return result.IsSuccess ? result : await asyncFallbackResult;
        }
#endif

#if NOPE_AWAITABLE
        // --- Awaitable ---

        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async Awaitable<Result<T, E>> Or<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Result<T, E> fallbackResult)
        {
            var result = await asyncResult;
            return result.Or(fallbackResult);
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async Awaitable<Result<T, E>> Or<T, E>(
            this Result<T, E> result,
            Awaitable<Result<T, E>> asyncFallbackResult)
        {
            return result.IsSuccess ? result : await asyncFallbackResult;
        }

        /// <summary>
        /// Returns the result if it is `Success`; otherwise returns the provided `fallbackResult`.
        /// </summary>
        public static async Awaitable<Result<T, E>> Or<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Awaitable<Result<T, E>> asyncFallbackResult)
        {
            var result = await asyncResult;
            return result.IsSuccess ? result : await asyncFallbackResult;
        }
#endif
    }
}