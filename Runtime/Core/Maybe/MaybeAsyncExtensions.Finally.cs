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
        // ------------------- UniTask-based Async Methods (FINALLY) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async action -> returns UniTask<Maybe<T>>
        ///    (equivalent to "Tap" but executed even if None?)
        /// </summary>
        public static async UniTask<Maybe<T>> Finally<T>(
            this Maybe<T> maybe,
            Func<Maybe<T>, UniTask> finalAction)
        {
            await finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// (B) Async UniTask<Maybe<T>> -> sync action -> returns UniTask<Maybe<T>>
        /// </summary>
        public static async UniTask<Maybe<T>> Finally<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Action<Maybe<T>> finalAction)
        {
            var maybe = await asyncMaybe;
            finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// (C) Async UniTask<Maybe<T>> -> async action -> returns UniTask<Maybe<T>>
        /// </summary>
        public static async UniTask<Maybe<T>> Finally<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, UniTask> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (FINALLY) -------------------

        /// <summary>
        /// (A) Sync Maybe<T> -> async action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Maybe<T> maybe,
            Func<Maybe<T>, Awaitable> finalAction)
        {
            await finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// (B) Async Awaitable<Maybe<T>> -> sync action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Action<Maybe<T>> finalAction)
        {
            var maybe = await asyncMaybe;
            finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// (C) Async Awaitable<Maybe<T>> -> async action -> returns Awaitable<Maybe<T>>
        /// </summary>
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, Awaitable> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
        }
#endif // NOPE_AWAITABLE

    }
}
