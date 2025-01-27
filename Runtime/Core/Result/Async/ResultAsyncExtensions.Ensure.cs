using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (ENSURE) -------------------
        public static async UniTask<Result<T>> Ensure<T>(
            this Result<T> result,
            Func<T, UniTask<bool>> asyncPredicate,
            string errorMessage)
        {
            if (result.IsSuccess)
            {
                bool valid = await asyncPredicate(result.Value);
                if (!valid)
                    return Result<T>.Failure(errorMessage);
            }
            return result;
        }

        public static async UniTask<Result<T>> Ensure<T>(
            this UniTask<Result<T>> asyncResult,
            Func<T, bool> predicate,
            string errorMessage)
        {
            var result = await asyncResult;
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T>.Failure(errorMessage);
            }
            return result;
        }
        
        public static async UniTask<Result<T>> Ensure<T>(
            this UniTask<Result<T>> asyncResult,
            Func<T, UniTask<bool>> predicateAsync,
            string errorMessage)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                bool valid = await predicateAsync(result.Value);
                if (!valid)
                    return Result<T>.Failure(errorMessage);
            }
            return result;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (ENSURE) -------------------
        public static async Awaitable<Result<T>> Ensure<T>(
            this Result<T> result,
            Func<T, Awaitable<bool>> asyncPredicate,
            string errorMessage)
        {
            if (result.IsSuccess)
            {
                bool valid = await asyncPredicate(result.Value);
                if (!valid)
                    return Result<T>.Failure(errorMessage);
            }
            return result;
        }

        public static async Awaitable<Result<T>> Ensure<T>(
            this Awaitable<Result<T>> asyncResult,
            Func<T, bool> predicate,
            string errorMessage)
        {
            var result = await asyncResult;
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T>.Failure(errorMessage);
            }
            return result;
        }
        
        public static async Awaitable<Result<T>> Ensure<T>(
            this Awaitable<Result<T>> asyncResult,
            Func<T, Awaitable<bool>> predicateAsync,
            string errorMessage)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                bool valid = await predicateAsync(result.Value);
                if (!valid)
                    return Result<T>.Failure(errorMessage);
            }
            return result;
        }
#endif // NOPE_AWAITABLE

    }
}
