using System;
using System.Collections.Generic;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeEnumerableExtensions
    {
        /// <summary>
        /// Tries to get the last element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static Maybe<T> TryLast<T>(
            this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var found = false;
            var last = default(T);

            foreach (var item in source)
            {
                found = true;
                last = item;
            }
            return found ? Maybe<T>.From(last) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static Maybe<T> TryLast<T>(
            this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
                if (predicate(item))
                    result = Maybe<T>.From(item);
            return result;
        }

#if NOPE_UNITASK
        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicateAsync">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryLast<T>(
            this IEnumerable<T> source,
            Func<T, UniTask<bool>> predicateAsync)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAsync == null) throw new ArgumentNullException(nameof(predicateAsync));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
            {
                if (await predicateAsync(item))
                    result = Maybe<T>.From(item);
            }
            return result;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicateAwaitable">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryLast<T>(
            this IEnumerable<T> source,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAwaitable == null) throw new ArgumentNullException(nameof(predicateAwaitable));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
            {
                if (await predicateAwaitable(item))
                    result = Maybe<T>.From(item);
            }
            return result;
        }
#endif


#if NOPE_UNITASK

        /// <summary>
        /// Tries to get the last element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static async UniTask<Maybe<T>> TryLast<T>(
            this UniTask<IEnumerable<T>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));

            var found = false;
            var last = default(T);

            foreach (var item in source)
            {
                found = true;
                last = item;
            }
            return found ? Maybe<T>.From(last) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryLast<T>(
            this UniTask<IEnumerable<T>> asyncSource,
            Func<T, bool> predicate)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
                if (predicate(item))
                    result = Maybe<T>.From(item);
            return result;
        }

        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicateAsync">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryLast<T>(
            this UniTask<IEnumerable<T>> asyncSource,
            Func<T, UniTask<bool>> predicateAsync)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAsync == null) throw new ArgumentNullException(nameof(predicateAsync));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
            {
                if (await predicateAsync(item))
                    result = Maybe<T>.From(item);
            }
            return result;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Tries to get the last element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static async Awaitable<Maybe<T>> TryLast<T>(
            this Awaitable<IEnumerable<T>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));

            var found = false;
            var last = default(T);

            foreach (var item in source)
            {
                found = true;
                last = item;
            }
            return found ? Maybe<T>.From(last) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryLast<T>(
            this Awaitable<IEnumerable<T>> asyncSource,
            Func<T, bool> predicate)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
                if (predicate(item))
                    result = Maybe<T>.From(item);
            return result;
        }

        /// <summary>
        /// Tries to get the last element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicateAwaitable">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryLast<T>(
            this Awaitable<IEnumerable<T>> asyncSource,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAwaitable == null) throw new ArgumentNullException(nameof(predicateAwaitable));

            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
            {
                if (await predicateAwaitable(item))
                    result = Maybe<T>.From(item);
            }
            return result;
        }
#endif
    }
}
