using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core
{
    public static class MaybeAsyncExtensions
    {
#if NOPE_UNITASK
        public static async UniTask<Maybe<TNew>> Map<TOriginal, TNew>(
            this UniTask<Maybe<TOriginal>> asyncMaybe,
            Func<TOriginal, TNew> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? Maybe<TNew>.From(selector(maybe.Value))
                : Maybe<TNew>.None;
        }

        public static async UniTask<Maybe<TNew>> Bind<TOriginal, TNew>(
            this UniTask<Maybe<TOriginal>> asyncMaybe,
            Func<TOriginal, UniTask<Maybe<TNew>>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? await binder(maybe.Value)
                : Maybe<TNew>.None;
        }

        public static async UniTask<Maybe<T>> Tap<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask> action)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
            {
                await action(maybe.Value);
            }
            return maybe;
        }

        public static async UniTask<TResult> Match<T, TResult>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<T, UniTask<TResult>> onValue,
            Func<UniTask<TResult>> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? await onValue(maybe.Value)
                : await onNone();
        }

        public static async UniTask<Maybe<T>> Finally<T>(
            this UniTask<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, UniTask> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
        }
#endif

#if NOPE_AWAITABLE
        public static async Awaitable<Maybe<TNew>> Map<TOriginal, TNew>(
            this Awaitable<Maybe<TOriginal>> asyncMaybe,
            Func<TOriginal, TNew> selector)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? Maybe<TNew>.From(selector(maybe.Value))
                : Maybe<TNew>.None;
        }
        
        public static async Awaitable<Maybe<TNew>> Bind<TOriginal, TNew>(
            this Awaitable<Maybe<TOriginal>> asyncMaybe,
            Func<TOriginal, Awaitable<Maybe<TNew>>> binder)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? await binder(maybe.Value)
                : Maybe<TNew>.None;
        }
        
        public static async Awaitable<Maybe<T>> Tap<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable> action)
        {
            var maybe = await asyncMaybe;
            if (maybe.HasValue)
            {
                await action(maybe.Value);
            }
            return maybe;
        }
        
        public static async Awaitable<TResult> Match<T, TResult>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<T, Awaitable<TResult>> onValue,
            Func<Awaitable<TResult>> onNone)
        {
            var maybe = await asyncMaybe;
            return maybe.HasValue
                ? await onValue(maybe.Value)
                : await onNone();
        }
        
        public static async Awaitable<Maybe<T>> Finally<T>(
            this Awaitable<Maybe<T>> asyncMaybe,
            Func<Maybe<T>, Awaitable> finalAction)
        {
            var maybe = await asyncMaybe;
            await finalAction(maybe);
            return maybe;
        }
#endif
    }
}
