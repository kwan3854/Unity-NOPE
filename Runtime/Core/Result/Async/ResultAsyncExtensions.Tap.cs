using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (TAP) -------------------
        public static async UniTask<Result<T>> Tap<T>(
            this Result<T> result,
            Func<T, UniTask> asyncAction)
        {
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }

        public static async UniTask<Result<T>> Tap<T>(
            this UniTask<Result<T>> asyncResult,
            Action<T> action)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }

        public static async UniTask<Result<T>> Tap<T>(
            this UniTask<Result<T>> asyncResult,
            Func<T, UniTask> asyncAction)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (TAP) -------------------
        public static async Awaitable<Result<T>> Tap<T>(
            this Result<T> result,
            Func<T, Awaitable> asyncAction)
        {
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }

        public static async Awaitable<Result<T>> Tap<T>(
            this Awaitable<Result<T>> asyncResult,
            Action<T> action)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }

        public static async Awaitable<Result<T>> Tap<T>(
            this Awaitable<Result<T>> asyncResult,
            Func<T, Awaitable> asyncAction)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }
#endif // NOPE_AWAITABLE

    }
}
