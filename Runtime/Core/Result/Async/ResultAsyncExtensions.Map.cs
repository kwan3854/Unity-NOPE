using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (MAP) -------------------
        public static async UniTask<Result<TNew>> Map<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, UniTask<TNew>> asyncSelector)
        {
            return result.IsSuccess
                ? Result<TNew>.Success(await asyncSelector(result.Value))
                : Result<TNew>.Failure(result.Error);
        }

        public static async UniTask<Result<TNew>> Map<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result<TNew>.Success(selector(result.Value))
                : Result<TNew>.Failure(result.Error);
        }

        public static async UniTask<Result<TNew>> Map<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<TNew>> asyncSelector)
        {
            var result = await asyncResult;
            if (result.IsFailure)
                return Result<TNew>.Failure(result.Error);

            var newVal = await asyncSelector(result.Value);
            return Result<TNew>.Success(newVal);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (MAP) -------------------
        public static async Awaitable<Result<TNew>> Map<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, Awaitable<TNew>> asyncSelector)
        {
            if (result.IsFailure) 
                return Result<TNew>.Failure(result.Error);

            var newVal = await asyncSelector(result.Value);
            return Result<TNew>.Success(newVal);
        }

        public static async Awaitable<Result<TNew>> Map<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result<TNew>.Success(selector(result.Value))
                : Result<TNew>.Failure(result.Error);
        }

        public static async Awaitable<Result<TNew>> Map<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<TNew>> asyncSelector)
        {
            var result = await asyncResult;
            if (result.IsFailure) 
                return Result<TNew>.Failure(result.Error);

            var newVal = await asyncSelector(result.Value);
            return Result<TNew>.Success(newVal);
        }
#endif // NOPE_AWAITABLE

    }
}
