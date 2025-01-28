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

#if NOPE_UNITASK

        /// <summary>
        /// Awaits an async Maybe and returns its value, or throws an exception if it has no value.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async UniTask<T> GetValueOrThrow<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Exception customException = null)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return maybe.Value;
            if (customException != null)
                throw customException;
            throw new InvalidOperationException("No value present in Maybe.");
        }

        /// <summary>
        /// Awaits an async Maybe and returns its value, or a provided default if no value.
        /// </summary>
        public static async UniTask<T> GetValueOrDefault<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            T defaultValue = default)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe.Value : defaultValue;
        }


        /// <summary>
        /// Asynchronously returns this Maybe if it has a value; if None, returns the fallback Maybe.
        /// </summary>
        public static async UniTask<Maybe<T>> Or<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Maybe<T> fallbackMaybe)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : fallbackMaybe;
        }

        /// <summary>
        /// Asynchronously returns this Maybe if it has a value; if None, creates a new Maybe from the fallbackValue.
        /// </summary>
        public static async UniTask<Maybe<T>> Or<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            T fallbackValue)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : Maybe<T>.From(fallbackValue);
        }
#endif

#if NOPE_AWAITABLE

        /// <summary>
        /// Awaits an async Maybe (Awaitable) and returns its value, or throws if no value.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Awaitable<T> GetValueOrThrow<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Exception customException = null)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
                return maybe.Value;
            if (customException != null)
                throw customException;
            throw new InvalidOperationException("No value present in Maybe.");
        }

        /// <summary>
        /// Awaits an async Maybe (Awaitable) and returns its value, or a default if no value.
        /// </summary>
        public static async Awaitable<T> GetValueOrDefault<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            T defaultValue = default)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe.Value : defaultValue;
        }

        /// <summary>
        /// Asynchronously returns this Maybe if it has a value; if None, returns the fallback Maybe.
        /// </summary>
        public static async Awaitable<Maybe<T>> Or<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Maybe<T> fallbackMaybe)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : fallbackMaybe;
        }

        /// <summary>
        /// Asynchronously returns this Maybe if it has a value; if None, creates a new Maybe from the fallbackValue.
        /// </summary>
        public static async Awaitable<Maybe<T>> Or<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            T fallbackValue)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? maybe : Maybe<T>.From(fallbackValue);
        }
#endif
    }
}