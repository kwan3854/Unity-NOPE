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
        // ------------------- UniTask-based Async Methods (MAP) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> Async selector -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Map<T, TNew>(
            this Maybe<T> maybe,
            Func<T, UniTask<TNew>> asyncSelector)
        {
            if (!maybe.HasValue)
                return Maybe<TNew>.None;
            var newValue = await asyncSelector(maybe.Value);
            return Maybe<TNew>.From(newValue);
        }

        /// <summary>
        /// (B) Async UniTask<Maybe<T>> -> Sync selector -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Map<T, TNew>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TNew> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? Maybe<TNew>.From(selector(maybe.Value))
                : Maybe<TNew>.None;
        }

        /// <summary>
        /// (C) Async UniTask<Maybe<T>> -> Async selector -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Map<T, TNew>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TNew>> asyncSelector)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            var newValue = await asyncSelector(maybe.Value);
            return Maybe<TNew>.From(newValue);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (MAP) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> Async selector -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Map<T, TNew>(
            this Maybe<T> maybe,
            Func<T, Awaitable<TNew>> asyncSelector)
        {
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            var newValue = await asyncSelector(maybe.Value);
            return Maybe<TNew>.From(newValue);
        }

        /// <summary>
        /// (B) Async Awaitable<Maybe<T>> -> Sync selector -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Map<T, TNew>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TNew> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? Maybe<TNew>.From(selector(maybe.Value))
                : Maybe<TNew>.None;
        }

        /// <summary>
        /// (C) Async Awaitable<Maybe<T>> -> Async selector -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Map<T, TNew>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TNew>> asyncSelector)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            var newValue = await asyncSelector(maybe.Value);
            return Maybe<TNew>.From(newValue);
        }
#endif // NOPE_AWAITABLE

    }
}
