using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    /// <summary>
    /// Provides all possible overloads for 'Where' method 
    /// in sync/async combinations (Maybe<T>, UniTask<Maybe<T>>, Awaitable<Maybe<T>>).
    /// </summary>
    public static partial class MaybeWhereExtensions
    {
        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static Maybe<T> Where<T>(
            this Maybe<T> maybe,
            Func<T, bool> predicate)
        {
            if (!maybe.HasValue) 
                return maybe;
            return predicate(maybe.Value) ? maybe : Maybe<T>.None;
        }

#if NOPE_UNITASK
        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this Maybe<T> maybe,
            Func<T, UniTask<bool>> predicateAsync)
        {
            if (!maybe.HasValue)
                return maybe;

            bool valid = await predicateAsync(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;

            return predicate(maybe.Value) ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<bool>> predicateAsync)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;

            bool valid = await predicateAsync(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Maybe<T> maybe,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            if (!maybe.HasValue)
                return maybe;

            bool valid = await predicateAwaitable(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;

            return predicate(maybe.Value) ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;

            bool valid = await predicateAwaitable(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }
#endif // NOPE_AWAITABLE

    }
}
