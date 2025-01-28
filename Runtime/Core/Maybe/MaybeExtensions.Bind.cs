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
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
        /// </summary>
        public static Maybe<TNew> Bind<TOriginal, TNew>(
            this Maybe<TOriginal> maybe,
            Func<TOriginal, Maybe<TNew>> binder)
        {
            return maybe.HasValue
                ? binder(maybe.Value)
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
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
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
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
        /// </summary>
        public static async UniTask<Maybe<TNew>> Bind<T, TNew>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TNew>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? binder(maybe.Value) : Maybe<TNew>.None;
        }

        /// <summary>
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
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
    }
}
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
namespace NOPE.Runtime.Core.Maybe.AwaitableAsync
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
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
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
        /// </summary>
        public static async Awaitable<Maybe<TNew>> Bind<T, TNew>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Maybe<TNew>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? binder(maybe.Value) : Maybe<TNew>.None;
        }

        /// <summary>
        /// Converts the inner value into another Maybe if present; otherwise returns None.
        /// (similar to Bind/flatMap)
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
    }
}
#endif // NOPE_AWAITABLE
