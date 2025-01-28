using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeUtilExtensions
    {
        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static Maybe<T> Where<T>(
            this Maybe<T> maybe,
            Func<T, bool> predicate)
        {
            if (maybe.HasValue && !predicate(maybe.Value))
            {
                return Maybe<T>.None;
            }
            return maybe;
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static Maybe<TResult> Select<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> selector)
        {
            return maybe.Map(selector);
        }

        /// <summary>
        /// For LINQ comprehension: same as Bind().
        /// </summary>
        public static Maybe<TResult> SelectMany<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TResult>> binder,
            Func<T, TResult, TResult> resultSelector)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

#if NOPE_UNITASK
        /// <summary>
        /// Awaits an async Maybe and applies a predicate function; returns None if it fails.
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

        /// <summary>
        /// Asynchronously evaluates a boolean predicate.
        /// If the Maybe has a value but does not satisfy the predicate, returns None.
        /// Otherwise, returns the same Maybe.
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue && !predicate(maybe.Value))
                return Maybe<T>.None;
            return maybe;
        }
        
        /// <summary>
        /// Asynchronously applies a selector function if the Maybe is not None.
        /// Returns a new Maybe of the transformed value.
        /// </summary>
        public static async UniTask<Maybe<TResult>> Select<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Maybe<TResult>.From(selector(maybe.Value)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously binds a Maybe using the provided binder function.
        /// If the Maybe is None, returns None.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<Maybe<TIntermediate>>> binder,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Awaits an async Maybe (Awaitable) and filters it by a predicate function.
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

        /// <summary>
        /// Asynchronously evaluates a boolean predicate.
        /// If the Maybe has a value but does not satisfy the predicate, returns None.
        /// Otherwise, returns the same Maybe.
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue && !predicate(maybe.Value))
                return Maybe<T>.None;
            return maybe;
        }
        
        /// <summary>
        /// Asynchronously applies a selector function if the Maybe is not None.
        /// Returns a new Maybe of the transformed value.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> Select<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Maybe<TResult>.From(selector(maybe.Value)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously binds a Maybe using the provided binder function.
        /// If the Maybe is None, returns None.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<Maybe<TIntermediate>>> binder,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif
    }
}
