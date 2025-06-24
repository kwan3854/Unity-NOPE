using System;
using NOPE.Runtime.Core.Result;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// If the Maybe has a value, returns it; otherwise executes the fallback function and returns its result.
        /// </summary>
        public static Maybe<T> OrElse<T>(
            this Maybe<T> maybe,
            Func<Maybe<T>> fallbackFunc)
        {
            return maybe.HasValue ? maybe : fallbackFunc();
        }

        /// <summary>
        /// If the Maybe has a value, returns it as a Result.Success; otherwise executes the fallback function and returns its result.
        /// </summary>
        public static Result<T, E> OrElse<T, E>(
            this Maybe<T> maybe,
            Func<Result<T, E>> fallbackFunc)
        {
            return maybe.HasValue
                ? Result<T, E>.Success(maybe.Value)
                : fallbackFunc();
        }

#if NOPE_UNITASK
        // --- UniTask-based Async Methods (OrElse) ---

        /// <summary>
        /// Asynchronously returns the maybe if it has a value, otherwise executes the synchronous fallback function.
        /// </summary>
        public static async UniTask<Maybe<T>> OrElse<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<Maybe<T>> fallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Asynchronously returns the maybe if it has a value, otherwise executes the asynchronous fallback function.
        /// </summary>
        public static async UniTask<Maybe<T>> OrElse<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<UniTask<Maybe<T>>> asyncFallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : await asyncFallbackFunc();
        }
        
        /// <summary>
        /// Returns the maybe if it has a value, otherwise executes the asynchronous fallback function.
        /// </summary>
        public static UniTask<Maybe<T>> OrElse<T>(
            this Maybe<T> maybe,
            Func<UniTask<Maybe<T>>> asyncFallbackFunc)
        {
            return maybe.HasValue ? UniTask.FromResult(maybe) : asyncFallbackFunc();
        }

        /// <summary>
        /// Asynchronously converts the maybe to a Result, or executes the synchronous fallback function if it has no value.
        /// </summary>
        public static async UniTask<Result<T, E>> OrElse<T, E>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<Result<T, E>> fallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Asynchronously converts the maybe to a Result, or executes the asynchronous fallback function if it has no value.
        /// </summary>
        public static async UniTask<Result<T, E>> OrElse<T, E>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<UniTask<Result<T, E>>> asyncFallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Result<T, E>.Success(maybe.Value) : await asyncFallbackFunc();
        }
        
        /// <summary>
        /// Converts the maybe to a Result, or executes the asynchronous fallback function if it has no value.
        /// </summary>
        public static UniTask<Result<T, E>> OrElse<T, E>(
            this Maybe<T> maybe,
            Func<UniTask<Result<T, E>>> asyncFallbackFunc)
        {
            return maybe.HasValue ? UniTask.FromResult(Result<T, E>.Success(maybe.Value)) : asyncFallbackFunc();
        }
#endif

#if NOPE_AWAITABLE
        // --- Awaitable-based Async Methods (OrElse) ---

        /// <summary>
        /// Asynchronously returns the maybe if it has a value, otherwise executes the synchronous fallback function.
        /// </summary>
        public static async Awaitable<Maybe<T>> OrElse<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Maybe<T>> fallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Asynchronously returns the maybe if it has a value, otherwise executes the asynchronous fallback function.
        /// </summary>
        public static async Awaitable<Maybe<T>> OrElse<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Awaitable<Maybe<T>>> asyncFallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : await asyncFallbackFunc();
        }
        
        /// <summary>
        /// Returns the maybe if it has a value, otherwise executes the asynchronous fallback function.
        /// </summary>
        public static async Awaitable<Maybe<T>> OrElse<T>(
            this Maybe<T> maybe,
            Func<Awaitable<Maybe<T>>> asyncFallbackFunc)
        {
            if (maybe.HasValue) return maybe;
            return await asyncFallbackFunc();
        }

        /// <summary>
        /// Asynchronously converts the maybe to a Result, or executes the synchronous fallback function if it has no value.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Result<T, E>> fallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.OrElse(fallbackFunc);
        }

        /// <summary>
        /// Asynchronously converts the maybe to a Result, or executes the asynchronous fallback function if it has no value.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Awaitable<Result<T, E>>> asyncFallbackFunc)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Result<T, E>.Success(maybe.Value) : await asyncFallbackFunc();
        }
        
        /// <summary>
        /// Converts the maybe to a Result, or executes the asynchronous fallback function if it has no value.
        /// </summary>
        public static async Awaitable<Result<T, E>> OrElse<T, E>(
            this Maybe<T> maybe,
            Func<Awaitable<Result<T, E>>> asyncFallbackFunc)
        {
            if (maybe.HasValue) return Result<T, E>.Success(maybe.Value);
            return await asyncFallbackFunc();
        }
#endif
    }
}