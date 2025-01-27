using System;
using NOPE.Runtime.Core;

namespace NOPE.Runtime.AdvancedExtensions
{
    public static class MaybeAdvancedExtensions
    {
        /// <summary>
        /// Returns the inner value if present; otherwise throws an InvalidOperationException,
        /// or the provided exception if given.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetValueOrThrow<T>(
            this Maybe<T> maybe,
            Exception customException = null)
        {
            if (maybe.HasValue) 
                return maybe.Value;

            if (customException != null) 
                throw customException;

            throw new InvalidOperationException("No value present in Maybe.");
        }

        /// <summary>
        /// Returns the inner value if present; otherwise returns the specified default value.
        /// </summary>
        public static T GetValueOrDefault<T>(
            this Maybe<T> maybe,
            T defaultValue = default)
        {
            return maybe.HasValue ? maybe.Value : defaultValue;
        }

        /// <summary>
        /// Returns this Maybe if it has a value; otherwise returns the fallbackMaybe.
        /// (Similar to the '??' operator logic)
        /// </summary>
        public static Maybe<T> Or<T>(
            this Maybe<T> maybe,
            Maybe<T> fallbackMaybe)
        {
            return maybe.HasValue ? maybe : fallbackMaybe;
        }

        /// <summary>
        /// Returns this Maybe if it has a value; otherwise creates a new Maybe from the fallbackValue.
        /// </summary>
        public static Maybe<T> Or<T>(
            this Maybe<T> maybe,
            T fallbackValue)
        {
            return maybe.HasValue ? maybe : Maybe<T>.From(fallbackValue);
        }

        /// <summary>
        /// Returns this Maybe if the predicate is true; otherwise returns None.
        /// (If the Maybe is already None, it just remains None.)
        /// </summary>
        public static Maybe<T> Where<T>(
            this Maybe<T> maybe,
            Func<T, bool> predicate)
        {
            if (maybe.HasValue && !predicate(maybe.Value))
            {
                return Maybe<T>.None;
            }
            return maybe;
        }

        /// <summary>
        /// For LINQ comprehension: same as Map().
        /// </summary>
        public static Maybe<TResult> Select<T, TResult>(
            this Maybe<T> maybe,
            Func<T, TResult> selector)
        {
            return maybe.Map(selector);
        }

        /// <summary>
        /// For LINQ comprehension: same as Bind().
        /// </summary>
        public static Maybe<TResult> SelectMany<T, TResult>(
            this Maybe<T> maybe,
            Func<T, Maybe<TResult>> binder)
        {
            return maybe.Bind(binder);
        }
    }
}