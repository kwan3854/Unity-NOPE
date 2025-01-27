using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (BIND) -------------------
        public static async UniTask<Result<TNew>> Bind<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, UniTask<Result<TNew>>> asyncBinder)
        {
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }

        public static async UniTask<Result<TNew>> Bind<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, Result<TNew>> binder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }

        public static async UniTask<Result<TNew>> Bind<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<Result<TNew>>> asyncBinder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (BIND) -------------------
        public static async Awaitable<Result<TNew>> Bind<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, Awaitable<Result<TNew>>> asyncBinder)
        {
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }

        public static async Awaitable<Result<TNew>> Bind<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Result<TNew>> binder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }

        public static async Awaitable<Result<TNew>> Bind<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<Result<TNew>>> asyncBinder)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await asyncBinder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }
#endif // NOPE_AWAITABLE

    }
}
