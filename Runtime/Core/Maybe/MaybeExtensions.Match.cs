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
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static TResult Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> onValue,
            Func<TResult> onNone)
        {
            return maybe.HasValue
                ? onValue(maybe.Value)
                : onNone();
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Maybe.UniTaskAsync
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> onValue,
            Func<UniTask<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return onValue(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<TResult> onNone)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<UniTask<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? onValue(maybe.Value)
                : onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<UniTask<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return onValue(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TResult>> onValueAsync,
            Func<UniTask<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
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
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> onValue,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return onValue(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<TResult> onNone)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? onValue(maybe.Value)
                : onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> onValue,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return onValue(maybe.Value);
            else
                return await onNoneAsync();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<TResult> onNone)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return onNone();
        }

        /// <summary>
        /// Converts a Maybe to a single value by providing two functions:
        /// one for when there is a value, and one for when there is not.
        /// </summary>
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TResult>> onValueAsync,
            Func<Awaitable<TResult>> onNoneAsync)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return await onValueAsync(maybe.Value);
            else
                return await onNoneAsync();
        }
    }
}
#endif // NOPE_AWAITABLE
