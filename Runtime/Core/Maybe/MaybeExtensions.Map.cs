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
        /// Projects the inner value to a new form if present; otherwise returns None.
        /// </summary>
        public static Maybe<TNew> Map<TOriginal, TNew>(
            this Maybe<TOriginal> maybe,
            Func<TOriginal, TNew> selector)
        {
            return maybe.HasValue
                ? Maybe<TNew>.From(selector(maybe.Value))
                : Maybe<TNew>.None;
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Maybe.UniTaskAsync
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Projects the inner value to a new form if present; otherwise returns None.
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
        /// Projects the inner value to a new form if present; otherwise returns None.
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
        /// Projects the inner value to a new form if present; otherwise returns None.
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
    }
}
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
namespace NOPE.Runtime.Core.Maybe.AwaitableAsync
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Projects the inner value to a new form if present; otherwise returns None.
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
        /// Projects the inner value to a new form if present; otherwise returns None.
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
        /// Projects the inner value to a new form if present; otherwise returns None.
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
    }
}
#endif // NOPE_AWAITABLE
