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
        // ------------------- UniTask-based Async Methods (TAP) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async action -> returns UniTask<Maybe<T>>
        /// </summary>
        public static async UniTask<Maybe<T>> Tap<T>(
            this Maybe<T> maybe,
            Func<T, UniTask> asyncAction)
        {
            if (maybe.HasValue)
                await asyncAction(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// (B) Async UniTask<Maybe<T>> -> sync action -> returns UniTask<Maybe<T>>
        /// </summary>
        public static async UniTask<Maybe<T>> Tap<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Action<T> action)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                action(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// (C) Async UniTask<Maybe<T>> -> async action -> returns UniTask<Maybe<T>>
        /// </summary>
        public static async UniTask<Maybe<T>> Tap<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask> asyncAction)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                await asyncAction(maybe.Value);
            return maybe;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (TAP) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Tap<T>(
            this Maybe<T> maybe,
            Func<T, Awaitable> asyncAction)
        {
            if (maybe.HasValue)
                await asyncAction(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// (B) Async Awaitable<Maybe<T>> -> sync action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Tap<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Action<T> action)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                action(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// (C) Async Awaitable<Maybe<T>> -> async action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Tap<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable> asyncAction)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                await asyncAction(maybe.Value);
            return maybe;
        }
#endif // NOPE_AWAITABLE

    }
}
