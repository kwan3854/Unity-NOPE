using System;
using UnityEngine;
#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (MATCH) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async onValue / async onNone -> returns UniTask<TResult>
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<UniTask<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// (B) Async UniTask<Maybe<T>> -> sync onValue / sync onNone -> returns UniTask<TResult>
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? onValue(maybe.Value)
                : onNone();
        }

        /// <summary>
        /// (C) Async UniTask<Maybe<T>> -> async onValue / async onNone -> returns UniTask<TResult>
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<UniTask<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (MATCH) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async onValue / async onNone -> returns Awaitable<TResult>
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// (B) Async Awaitable<Maybe<T>> -> sync onValue / sync onNone -> returns Awaitable<TResult>
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? onValue(maybe.Value)
                : onNone();
        }

        /// <summary>
        /// (C) Async Awaitable<Maybe<T>> -> async onValue / async onNone -> returns Awaitable<TResult>
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }
#endif // NOPE_AWAITABLE

    }
}
