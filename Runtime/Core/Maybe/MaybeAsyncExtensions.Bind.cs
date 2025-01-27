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
        // ------------------- UniTask-based Async Methods (BIND) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> Async binder -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Bind<T, TNew>(
            this Maybe<T> maybe,
            Func<T, UniTask<Maybe<TNew>>> asyncBinder)
        {
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            return await asyncBinder(maybe.Value);
        }

        /// <summary>
        /// (B) Async UniTask<Maybe<T>> -> Sync binder -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Bind<T, TNew>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TNew>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? binder(maybe.Value) : Maybe<TNew>.None;
        }

        /// <summary>
        /// (C) Async UniTask<Maybe<T>> -> Async binder -> returns UniTask<Maybe<TNew>>
        /// </summary>
        public static async UniTask<Maybe<TNew>> Bind<T, TNew>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<Maybe<TNew>>> asyncBinder)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            return await asyncBinder(maybe.Value);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (BIND) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> Async binder -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Bind<T, TNew>(
            this Maybe<T> maybe,
            Func<T, Awaitable<Maybe<TNew>>> asyncBinder)
        {
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            return await asyncBinder(maybe.Value);
        }

        /// <summary>
        /// (B) Async Awaitable<Maybe<T>> -> Sync binder -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Bind<T, TNew>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TNew>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? binder(maybe.Value) : Maybe<TNew>.None;
        }

        /// <summary>
        /// (C) Async Awaitable<Maybe<T>> -> Async binder -> returns Awaitable<Maybe<TNew>>
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Bind<T, TNew>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<Maybe<TNew>>> asyncBinder)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return Maybe<TNew>.None;

            return await asyncBinder(maybe.Value);
        }
#endif // NOPE_AWAITABLE

    }
}
