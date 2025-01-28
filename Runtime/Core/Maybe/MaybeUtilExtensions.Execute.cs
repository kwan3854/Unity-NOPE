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
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static Maybe<T> Execute<T>(
            this Maybe<T> maybe,
            Action<T> action)
        {
            if (maybe.HasValue)
                action(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static Maybe<T> ExecuteNoValue<T>(
            this Maybe<T> maybe,
            Action action)
        {
            if (maybe.HasNoValue)
                action();
            return maybe;
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Maybe.UniTaskAsync
{
    public static partial class MaybeUtilExtensions
    {
        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAsync">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> Execute<T>(
            this Maybe<T> maybe,
            Func<T, UniTask> actionAsync)
        {
            if (maybe.HasValue)
                await actionAsync(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAsync">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> ExecuteNoValue<T>(
            this Maybe<T> maybe,
            Func<UniTask> actionAsync)
        {
            if (maybe.HasNoValue)
                await actionAsync();
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> Execute<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Action<T> action)
        {
            var m = await asyncMaybe;
            if (m.HasValue)
                action(m.Value);
            return m;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> ExecuteNoValue<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Action action)
        {
            var m = await asyncMaybe;
            if (m.HasNoValue)
                action();
            return m;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAsync">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> Execute<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask> actionAsync)
        {
            var m = await asyncMaybe;
            if (m.HasValue)
                await actionAsync(m.Value);
            return m;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAsync">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async UniTask<Maybe<T>> ExecuteNoValue<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<UniTask> actionAsync)
        {
            var m = await asyncMaybe;
            if (m.HasNoValue)
                await actionAsync();
            return m;
        }
    }
}
#endif

#if NOPE_AWAITABLE
namespace NOPE.Runtime.Core.Maybe.AwaitableAsync
{
    public static partial class MaybeUtilExtensions
    {
        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAwaitable">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> Execute<T>(
            this Maybe<T> maybe,
            Func<T, Awaitable> actionAwaitable)
        {
            if (maybe.HasValue)
                await actionAwaitable(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAwaitable">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> ExecuteNoValue<T>(
            this Maybe<T> maybe,
            Func<Awaitable> actionAwaitable)
        {
            if (maybe.HasNoValue)
                await actionAwaitable();
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> Execute<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Action<T> action)
        {
            var m = await asyncMaybe;
            if (m.HasValue)
                action(m.Value);
            return m;
        }
        
        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> ExecuteNoValue<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Action action)
        {
            var m = await asyncMaybe;
            if (m.HasNoValue)
                action();
            return m;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAwaitable">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> Execute<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable> actionAwaitable)
        {
            var m = await asyncMaybe;
            if (m.HasValue)
                await actionAwaitable(m.Value);
            return m;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="asyncMaybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="actionAwaitable">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static async Awaitable<Maybe<T>> ExecuteNoValue<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Awaitable> actionAwaitable)
        {
            var m = await asyncMaybe;
            if (m.HasNoValue)
                await actionAwaitable();
            return m;
        }
    }
}
#endif
