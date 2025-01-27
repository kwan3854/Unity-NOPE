using System;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// Provides extension methods for Maybe&lt;T&gt; to enable functional-style chaining.
    /// </summary>
    public static class MaybeExtensions
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

        /// <summary>
        /// Executes the given action if there is a value; otherwise does nothing.
        /// Returns the original maybe unmodified.
        /// </summary>
        public static Maybe<T> Tap<T>(
            this Maybe<T> maybe,
            Action<T> action)
        {
            if (maybe.HasValue)
            {
                action(maybe.Value);
            }
            return maybe;
        }

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