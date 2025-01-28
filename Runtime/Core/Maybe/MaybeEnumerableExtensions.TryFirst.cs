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
        /// Tries to get the first element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static Maybe<T> TryFirst<T>(
            this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var e = source.GetEnumerator();
            return e.MoveNext() ? Maybe<T>.From(e.Current) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static Maybe<T> TryFirst<T>(
            this IEnumerable<T> source,
            Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            foreach (var item in source)
                if (predicate(item))
                    return Maybe<T>.From(item);
            return Maybe<T>.None;
        }

#if NOPE_UNITASK
        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicateAsync">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryFirst<T>(
            this IEnumerable<T> source,
            Func<T, UniTask<bool>> predicateAsync)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAsync == null) throw new ArgumentNullException(nameof(predicateAsync));

            foreach (var item in source)
            {
                if (await predicateAsync(item))
                    return Maybe<T>.From(item);
            }
            return Maybe<T>.None;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicateAwaitable">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryFirst<T>(
            this IEnumerable<T> source,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAwaitable == null) throw new ArgumentNullException(nameof(predicateAwaitable));

            foreach (var item in source)
            {
                if (await predicateAwaitable(item))
                    return Maybe<T>.From(item);
            }
            return Maybe<T>.None;
        }
#endif


#if NOPE_UNITASK
        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryFirst<T>(
            this UniTask<IEnumerable<T>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var e = source.GetEnumerator();
            return e.MoveNext() ? Maybe<T>.From(e.Current) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryFirst<T>(
            this UniTask<IEnumerable<T>> asyncSource,
            Func<T, bool> predicate)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var item in source)
                if (predicate(item))
                    return Maybe<T>.From(item);
            return Maybe<T>.None;
        }
        
        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicateAsync">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async UniTask<Maybe<T>> TryFirst<T>(
            this UniTask<IEnumerable<T>> asyncSource,
            Func<T, UniTask<bool>> predicateAsync)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAsync == null) throw new ArgumentNullException(nameof(predicateAsync));

            foreach (var item in source)
                if (await predicateAsync(item))
                    return Maybe<T>.From(item);
            return Maybe<T>.None;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryFirst<T>(
            this Awaitable<IEnumerable<T>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            using var e = source.GetEnumerator();
            return e.MoveNext() ? Maybe<T>.From(e.Current) : Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryFirst<T>(
            this Awaitable<IEnumerable<T>> asyncSource,
            Func<T, bool> predicate)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            foreach (var item in source)
                if (predicate(item))
                    return Maybe<T>.From(item);
            return Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the first element of the sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence to search.</param>
        /// <param name="predicateAwaitable">A function to test each element for a condition.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element that satisfies the condition if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or predicate is null.</exception>
        public static async Awaitable<Maybe<T>> TryFirst<T>(
            this Awaitable<IEnumerable<T>> asyncSource,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            var source = await asyncSource;
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicateAwaitable == null) throw new ArgumentNullException(nameof(predicateAwaitable));

            foreach (var item in source)
                if (await predicateAwaitable(item))
                    return Maybe<T>.From(item);
            return Maybe<T>.None;
        }
#endif
    }
}
