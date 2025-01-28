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
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
        /// </summary>
        public static Maybe<T> Finally<T>(
            this Maybe<T> maybe,
            Action<Maybe<T>> finalAction)
        {
            finalAction(maybe);
            return maybe;
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Maybe.UniTaskAsync
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
        /// </summary>
        public static async UniTask<Maybe<T>> Finally<T>(
            this Maybe<T> maybe,
            Func<Maybe<T>, UniTask> finalAction)
        {
            await finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
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
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
        /// </summary>
        public static async UniTask<Maybe<T>> Finally<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, UniTask> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
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
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
        /// </summary>
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Maybe<T> maybe,
            Func<Maybe<T>, Awaitable> finalAction)
        {
            await finalAction(maybe);
            return maybe;
        }

        /// <summary>
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
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
        /// Executes an action regardless of whether the Maybe has a value or not.
        /// Returns the original Maybe.
        /// </summary>
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, Awaitable> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
        }
    }
}
#endif // NOPE_AWAITABLE
