using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static Result<TNew, E> MapSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, TNew> selector,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? Result.Of(() => selector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }
        
#if NOPE_UNITASK
        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, UniTask<TNew>> asyncSelector,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? await Result.Of(async () => await asyncSelector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, TNew> selector,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result.Of(() => selector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async UniTask<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, UniTask<TNew>> asyncSelector,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await Result.Of(async () => await asyncSelector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Awaitable<TNew>> asyncSelector,
            Func<Exception, E> errorHandler)
        {
            return result.IsSuccess
                ? await Result.Of(async () => await asyncSelector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, TNew> selector,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result.Of(() => selector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>
        /// Maps the value of a successful Result into a new form.
        /// safely handling exceptions that may occur during the mapping process.
        /// </summary>
        /// <returns>A new Result containing the bound value or an error if an exception occurred.</returns>
        public static async Awaitable<Result<TNew, E>> MapSafe<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Awaitable<TNew>> asyncSelector,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await Result.Of(async () => await asyncSelector(result.Value), errorHandler)
                : Result<TNew, E>.Failure(result.Error);
        }
#endif
    }
}
