using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    /// <summary>
    /// Provides all possible overloads for 'Select' method 
    /// in sync/async combinations.
    /// </summary>
    public static partial class MaybeSelectExtensions
    {
        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static Maybe<TResult> Select<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> selector)
        {
            return maybe.Map(selector);
        }

#if NOPE_UNITASK
        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async UniTask<Maybe<TResult>> Select<T, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<TResult>> selectorAsync)
        {
            return await maybe.Map(selectorAsync);
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async UniTask<Maybe<TResult>> Select<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            return await asyncMaybe.Map(selector);
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async UniTask<Maybe<TResult>> Select<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TResult>> selectorAsync)
        {
            return await asyncMaybe.Map(selectorAsync);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async Awaitable<Maybe<TResult>> Select<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<TResult>> selectorAwaitable)
        {
            return await maybe.Map(selectorAwaitable);
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async Awaitable<Maybe<TResult>> Select<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            return await asyncMaybe.Map(selector);
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static async Awaitable<Maybe<TResult>> Select<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TResult>> selectorAwaitable)
        {
            return await asyncMaybe.Map(selectorAwaitable);
        }
#endif // NOPE_AWAITABLE

    }
}
