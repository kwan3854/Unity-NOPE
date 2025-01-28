using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    /// <summary>
    /// Provides all possible overloads for 'SelectMany' method 
    /// in sync/async binder + sync/async resultSelector combinations.
    /// </summary>
    public static partial class MaybeSelectManyExtensions
    {
        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static Maybe<TResult> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, TResult> resultSelector)
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
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<Maybe<TIntermediate>>> binderAsync,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAsync(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, UniTask<TResult>> resultSelectorAsync)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAsync(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<Maybe<TIntermediate>>> binderAsync,
            Func<T, TIntermediate, UniTask<TResult>> resultSelectorAsync)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAsync(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAsync(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<Maybe<TIntermediate>>> binderAwaitable,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAwaitable(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, Awaitable<TResult>> resultSelectorAwaitable)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAwaitable(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<Maybe<TIntermediate>>> binderAwaitable,
            Func<T, TIntermediate, Awaitable<TResult>> resultSelectorAwaitable)
        {
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAwaitable(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAwaitable(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif // NOPE_AWAITABLE

        // ========================
        // 2) this UniTask<Maybe<T>>
        // ========================
#if NOPE_UNITASK
        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<Maybe<TIntermediate>>> binderAsync,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAsync(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, UniTask<TResult>> resultSelectorAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAsync(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<Maybe<TIntermediate>>> binderAsync,
            Func<T, TIntermediate, UniTask<TResult>> resultSelectorAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAsync(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAsync(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<Maybe<TIntermediate>>> binderAwaitable,
            Func<T, TIntermediate, TResult> resultSelector)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAwaitable(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(resultSelector(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TIntermediate>> binder,
            Func<T, TIntermediate, Awaitable<TResult>> resultSelectorAwaitable)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = binder(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAwaitable(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }

        /// <summary>
        /// Projects each element of a sequence to an intermediate sequence and flattens the resulting sequences into one sequence.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TIntermediate, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<Maybe<TIntermediate>>> binderAwaitable,
            Func<T, TIntermediate, Awaitable<TResult>> resultSelectorAwaitable)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasNoValue)
                return Maybe<TResult>.None;

            var intermediate = await binderAwaitable(maybe.Value);
            return intermediate.HasValue
                ? Maybe<TResult>.From(await resultSelectorAwaitable(maybe.Value, intermediate.Value))
                : Maybe<TResult>.None;
        }
#endif // NOPE_AWAITABLE

    }
}
