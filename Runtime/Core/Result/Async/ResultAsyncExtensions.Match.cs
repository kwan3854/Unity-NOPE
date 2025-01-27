using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ========================================================================
        // (A) 결과가 "동기" Result<T> 인 경우
        // ========================================================================
        
        // /// <summary>
        // /// (1a) Sync result + Sync onSuccess + Sync onFailure
        // /// </summary>
        // public static TResult Match<TOriginal, TResult>(
        //     this Result<TOriginal> result,
        //     Func<TOriginal, TResult> onSuccess,
        //     Func<string, TResult> onFailure)
        // {
        //     return result.IsSuccess
        //         ? onSuccess(result.Value)
        //         : onFailure(result.Error);
        // }

        /// <summary>
        /// (1b) Sync result + Sync onSuccess + Async onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, TResult> onSuccess,
            Func<string, UniTask<TResult>> onFailureAsync)
        {
            if (result.IsSuccess)
                return onSuccess(result.Value);
            else
                return await onFailureAsync(result.Error);
        }

        /// <summary>
        /// (1c) Sync result + Async onSuccess + Sync onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, UniTask<TResult>> onSuccessAsync,
            Func<string, TResult> onFailure)
        {
            if (result.IsSuccess)
                return await onSuccessAsync(result.Value);
            else
                return onFailure(result.Error);
        }

        /// <summary>
        /// (1d) Sync result + Async onSuccess + Async onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, UniTask<TResult>> onSuccessAsync,
            Func<string, UniTask<TResult>> onFailureAsync)
        {
            if (result.IsSuccess)
                return await onSuccessAsync(result.Value);
            else
                return await onFailureAsync(result.Error);
        }


        // ========================================================================
        // (B) 결과가 "비동기" UniTask<Result<T>> 인 경우
        // ========================================================================

        /// <summary>
        /// (2a) Async result + Sync onSuccess + Sync onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, TResult> onSuccess,
            Func<string, TResult> onFailure)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// (2b) Async result + Sync onSuccess + Async onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, TResult> onSuccess,
            Func<string, UniTask<TResult>> onFailureAsync)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return onSuccess(result.Value);
            else
                return await onFailureAsync(result.Error);
        }

        /// <summary>
        /// (2c) Async result + Async onSuccess + Sync onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<TResult>> onSuccessAsync,
            Func<string, TResult> onFailure)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAsync(result.Value);
            else
                return onFailure(result.Error);
        }

        /// <summary>
        /// (2d) Async result + Async onSuccess + Async onFailure
        /// </summary>
        public static async UniTask<TResult> Match<TOriginal, TResult>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<TResult>> onSuccessAsync,
            Func<string, UniTask<TResult>> onFailureAsync)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAsync(result.Value);
            else
                return await onFailureAsync(result.Error);
        }
#endif // NOPE_UNITASK
        
#if NOPE_AWAITABLE
        // ========================================================================
        // (A) 결과가 "동기" Result<T> 인 경우
        // ========================================================================

        // /// <summary>
        // /// (1a) Sync result + Sync onSuccess + Sync onFailure
        // /// </summary>
        // public static TResult Match<TOriginal, TResult>(
        //     this Result<TOriginal> result,
        //     Func<TOriginal, TResult> onSuccess,
        //     Func<string, TResult> onFailure)
        // {
        //     return result.IsSuccess
        //         ? onSuccess(result.Value)
        //         : onFailure(result.Error);
        // }

        /// <summary>
        /// (1b) Sync result + Sync onSuccess + Async onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, TResult> onSuccess,
            Func<string, Awaitable<TResult>> onFailureAwaitable)
        {
            if (result.IsSuccess)
                return onSuccess(result.Value);
            else
                return await onFailureAwaitable(result.Error);
        }

        /// <summary>
        /// (1c) Sync result + Async onSuccess + Sync onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, Awaitable<TResult>> onSuccessAwaitable,
            Func<string, TResult> onFailure)
        {
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            else
                return onFailure(result.Error);
        }

        /// <summary>
        /// (1d) Sync result + Async onSuccess + Async onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Result<TOriginal> result,
            Func<TOriginal, Awaitable<TResult>> onSuccessAwaitable,
            Func<string, Awaitable<TResult>> onFailureAwaitable)
        {
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            else
                return await onFailureAwaitable(result.Error);
        }


        // ========================================================================
        // (B) 결과가 "비동기" Awaitable<Result<T>> 인 경우
        // ========================================================================

        /// <summary>
        /// (2a) Async result + Sync onSuccess + Sync onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, TResult> onSuccess,
            Func<string, TResult> onFailure)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// (2b) Async result + Sync onSuccess + Async onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, TResult> onSuccess,
            Func<string, Awaitable<TResult>> onFailureAwaitable)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return onSuccess(result.Value);
            else
                return await onFailureAwaitable(result.Error);
        }

        /// <summary>
        /// (2c) Async result + Async onSuccess + Sync onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<TResult>> onSuccessAwaitable,
            Func<string, TResult> onFailure)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            else
                return onFailure(result.Error);
        }

        /// <summary>
        /// (2d) Async result + Async onSuccess + Async onFailure
        /// </summary>
        public static async Awaitable<TResult> Match<TOriginal, TResult>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<TResult>> onSuccessAwaitable,
            Func<string, Awaitable<TResult>> onFailureAwaitable)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            else
                return await onFailureAwaitable(result.Error);
        }
#endif // NOPE_AWAITABLE
    }
}
