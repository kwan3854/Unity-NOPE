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
        /// Filters a sequence of <see cref="Maybe{T}"/> values and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static IEnumerable<T> Choose<T>(
            this IEnumerable<Maybe<T>> source)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));

            foreach (var maybe in source)
                if (maybe.HasValue)
                    yield return maybe.Value;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="source">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selector">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static IEnumerable<TResult> Choose<T, TResult>(
            this IEnumerable<Maybe<T>> source,
            Func<T, TResult> selector)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            foreach (var maybe in source)
                if (maybe.HasValue)
                    yield return selector(maybe.Value);
        }

#if NOPE_UNITASK
        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="source">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selectorAsync">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async UniTask<IEnumerable<TResult>> Choose<T, TResult>(
            this IEnumerable<Maybe<T>> source,
            Func<T, UniTask<TResult>> selectorAsync)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            if (selectorAsync == null)
                throw new ArgumentNullException(nameof(selectorAsync));

            var results = new List<TResult>();
            foreach (var maybe in source)
            {
                if (maybe.HasValue)
                {
                    var transformed = await selectorAsync(maybe.Value);
                    results.Add(transformed);
                }
            }
            return results;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="source">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selectorAwaitable">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async Awaitable<IEnumerable<TResult>> Choose<T, TResult>(
            this IEnumerable<Maybe<T>> source,
            Func<T, Awaitable<TResult>> selectorAwaitable)
        {
            if (source == null) 
                throw new ArgumentNullException(nameof(source));
            if (selectorAwaitable == null)
                throw new ArgumentNullException(nameof(selectorAwaitable));

            var results = new List<TResult>();
            foreach (var maybe in source)
            {
                if (maybe.HasValue)
                {
                    var transformed = await selectorAwaitable(maybe.Value);
                    results.Add(transformed);
                }
            }
            return results;
        }
#endif
        
#if NOPE_UNITASK
        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static async UniTask<IEnumerable<T>> Choose<T>(
            this UniTask<IEnumerable<Maybe<T>>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var results = new List<T>();
            foreach (var maybe in source)
                if (maybe.HasValue)
                    results.Add(maybe.Value);

            return results;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selector">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async UniTask<IEnumerable<TResult>> Choose<T, TResult>(
            this UniTask<IEnumerable<Maybe<T>>> asyncSource,
            Func<T, TResult> selector)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var results = new List<TResult>();
            foreach (var maybe in source)
                if (maybe.HasValue)
                    results.Add(selector(maybe.Value));
            return results;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selectorAsync">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async UniTask<IEnumerable<TResult>> Choose<T, TResult>(
            this UniTask<IEnumerable<Maybe<T>>> asyncSource,
            Func<T, UniTask<TResult>> selectorAsync)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selectorAsync == null)
                throw new ArgumentNullException(nameof(selectorAsync));

            var results = new List<TResult>();
            foreach (var maybe in source)
            {
                if (maybe.HasValue)
                {
                    var transformed = await selectorAsync(maybe.Value);
                    results.Add(transformed);
                }
            }
            return results;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static async Awaitable<IEnumerable<T>> Choose<T>(
            this Awaitable<IEnumerable<Maybe<T>>> asyncSource)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var results = new List<T>();
            foreach (var maybe in source)
                if (maybe.HasValue)
                    results.Add(maybe.Value);

            return results;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selector">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async Awaitable<IEnumerable<TResult>> Choose<T, TResult>(
            this Awaitable<IEnumerable<Maybe<T>>> asyncSource,
            Func<T, TResult> selector)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var results = new List<TResult>();
            foreach (var maybe in source)
                if (maybe.HasValue)
                    results.Add(selector(maybe.Value));
            return results;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values, projects each value into a new form, and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by the selector.</typeparam>
        /// <param name="asyncSource">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <param name="selectorAwaitable">A transform function to apply to each value.</param>
        /// <returns>An <see cref="IEnumerable{TResult}"/> containing the transformed values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence or selector is null.</exception>
        public static async Awaitable<IEnumerable<TResult>> Choose<T, TResult>(
            this Awaitable<IEnumerable<Maybe<T>>> asyncSource,
            Func<T, Awaitable<TResult>> selectorAwaitable)
        {
            var source = await asyncSource;
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (selectorAwaitable == null)
                throw new ArgumentNullException(nameof(selectorAwaitable));

            var results = new List<TResult>();
            foreach (var maybe in source)
            {
                if (maybe.HasValue)
                {
                    var transformed = await selectorAwaitable(maybe.Value);
                    results.Add(transformed);
                }
            }
            return results;
        }
#endif
    }
}
