using System;
using System.Collections.Generic;

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeUtilExtensions
    {
        /// <summary>
        /// Tries to find a value in the dictionary by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dict">The dictionary to search.</param>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the value if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the dictionary is null.</exception>
        public static Maybe<TValue> TryFind<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            return dict.TryGetValue(key, out var value) ? Maybe<TValue>.From(value) : Maybe<TValue>.None;
        }

        /// <summary>
        /// Tries to get the first element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the first element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static Maybe<T> TryFirst<T>(this IEnumerable<T> source)
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
        public static Maybe<T> TryFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var item in source)
                if (predicate(item)) return Maybe<T>.From(item);
            return Maybe<T>.None;
        }

        /// <summary>
        /// Tries to get the last element of the sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the last element if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static Maybe<T> TryLast<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            T last = default;
            bool found = false;
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
        public static Maybe<T> TryLast<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            Maybe<T> result = Maybe<T>.None;
            foreach (var item in source)
                if (predicate(item)) result = Maybe<T>.From(item);
            return result;
        }

        /// <summary>
        /// Filters a sequence of <see cref="Maybe{T}"/> values and returns only those that have a value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The sequence of <see cref="Maybe{T}"/> values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing the values that are present.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source sequence is null.</exception>
        public static IEnumerable<T> Choose<T>(this IEnumerable<Maybe<T>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var maybe in source)
                if (maybe.HasValue) yield return maybe.Value;
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
        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<Maybe<T>> source, Func<T, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            foreach (var maybe in source)
                if (maybe.HasValue) yield return selector(maybe.Value);
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has a value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static Maybe<T> Execute<T>(this Maybe<T> maybe, Action<T> action)
        {
            if (maybe.HasValue) action(maybe.Value);
            return maybe;
        }

        /// <summary>
        /// Executes an action if the <see cref="Maybe{T}"/> has no value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="maybe">The <see cref="Maybe{T}"/> instance.</param>
        /// <param name="action">The action to execute if the value is not present.</param>
        /// <returns>The original <see cref="Maybe{T}"/> instance.</returns>
        public static Maybe<T> ExecuteNoValue<T>(this Maybe<T> maybe, Action action)
        {
            if (maybe.HasNoValue) action();
            return maybe;
        }
    }
}