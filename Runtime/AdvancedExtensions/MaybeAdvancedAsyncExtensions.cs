using System;
using Cysharp.Threading.Tasks;
using NOPE.Runtime.Core;
using UnityEngine;

namespace NOPE.Runtime.AdvancedExtensions
{
    /// <summary>
    /// Provides async extension methods for Maybe types using UniTask or Awaitable
    /// (depending on compilation symbols).
    /// </summary>
    public static class MaybeAdvancedAsyncExtensions
    {
#if NOPE_UNITASK

        /// <summary>
        /// Awaits an async Maybe and returns its value, or throws an exception if it has no value.
        /// </summary>
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
        /// Awaits an async Maybe and applies a predicate function; returns None if it fails.
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<bool>> predicateAsync)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;
            bool valid = await predicateAsync(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Asynchronously evaluates a boolean predicate.
        /// If the Maybe has a value but does not satisfy the predicate, returns None.
        /// Otherwise, returns the same Maybe.
        /// </summary>
        public static async UniTask<Maybe<T>> Where<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue && !predicate(maybe.Value))
                return Maybe<T>.None;
            return maybe;
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

        /// <summary>
        /// Asynchronously applies a selector function if the Maybe is not None.
        /// Returns a new Maybe of the transformed value.
        /// </summary>
        public static async UniTask<Maybe<TResult>> Select<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Maybe<TResult>.From(selector(maybe.Value)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously binds a Maybe using the provided binder function.
        /// If the Maybe is None, returns None.
        /// </summary>
        public static async UniTask<Maybe<TResult>> SelectMany<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<Maybe<TResult>>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? await binder(maybe.Value) : Maybe<TResult>.None;
        }
#endif

#if NOPE_AWAITABLE

        /// <summary>
        /// Awaits an async Maybe (Awaitable) and returns its value, or throws if no value.
        /// </summary>
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
        /// Awaits an async Maybe (Awaitable) and filters it by a predicate function.
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<bool>> predicateAwaitable)
        {
            var maybe = await asyncMaybe;
            if (!maybe.HasValue)
                return maybe;
            bool valid = await predicateAwaitable(maybe.Value);
            return valid ? maybe : Maybe<T>.None;
        }

        /// <summary>
        /// Asynchronously evaluates a boolean predicate.
        /// If the Maybe has a value but does not satisfy the predicate, returns None.
        /// Otherwise, returns the same Maybe.
        /// </summary>
        public static async Awaitable<Maybe<T>> Where<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, bool> predicate)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue && !predicate(maybe.Value))
                return Maybe<T>.None;
            return maybe;
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

        /// <summary>
        /// Asynchronously applies a selector function if the Maybe is not None.
        /// Returns a new Maybe of the transformed value.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> Select<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, TResult> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? Maybe<TResult>.From(selector(maybe.Value)) : Maybe<TResult>.None;
        }

        /// <summary>
        /// Asynchronously binds a Maybe using the provided binder function.
        /// If the Maybe is None, returns None.
        /// </summary>
        public static async Awaitable<Maybe<TResult>> SelectMany<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<Maybe<TResult>>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue ? await binder(maybe.Value) : Maybe<TResult>.None;
        }
#endif
    }
}