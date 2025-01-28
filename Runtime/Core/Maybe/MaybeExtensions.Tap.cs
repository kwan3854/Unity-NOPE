using System;
using UnityEngine;
#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
        /// </summary>
        public static Maybe<T> Tap<T>(
            this Maybe<T> maybe,
            Action<T> action)
        {
            if (maybe.HasValue)
            {
                action(maybe.Value);
            }
            return maybe;
        }
        
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (TAP) -------------------

        /// <summary>
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
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
