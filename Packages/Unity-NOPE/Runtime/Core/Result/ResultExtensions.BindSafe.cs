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
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static Result<TNew, E> BindSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Result<TNew, E>> binder,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? Result.Of(() => binder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }
        
#if NOPE_UNITASK
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, UniTask<Result<TNew, E>>> asyncBinder,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? await Result.Of(async () => await asyncBinder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Result<TNew, E>> binder,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result.Of(() => binder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, UniTask<Result<TNew, E>>> asyncBinder,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await Result.Of(async () => await asyncBinder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Awaitable<Result<TNew, E>>> asyncBinder,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? await Result.Of(async () => await asyncBinder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }
        
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Result<TNew, E>> binder,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result.Of(() => binder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }
        
        /// <summary>
        /// Flat-maps (binds) the value of a successful Result into another Result,
        /// safely handling exceptions that may occur during the binding process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> BindSafe<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Awaitable<Result<TNew, E>>> asyncBinder,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await Result.Of(async () => await asyncBinder(result.Value), errorHandler).Bind(x => x)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif
    }
}
