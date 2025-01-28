using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif


namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static Result<TNew, E> Bind<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Result<TNew, E>> binder)
        {
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

#if NOPE_UNITASK
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async UniTask<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, UniTask<Result<TNew, E>>> asyncBinder)
        {
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async UniTask<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Result<TNew, E>> binder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async UniTask<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, UniTask<Result<TNew, E>>> asyncBinder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async Awaitable<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Awaitable<Result<TNew, E>>> asyncBinder)
        {
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async Awaitable<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Result<TNew, E>> binder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result.
        /// </summary>
        public static async Awaitable<Result<TNew, E>> Bind<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Awaitable<Result<TNew, E>>> asyncBinder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif
    }
}
