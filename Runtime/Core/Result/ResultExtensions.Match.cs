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
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static TResult Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, TResult> onSuccess,
            Func<E, TResult> onFailure)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Result.UniTaskAsync
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, TResult> onSuccess,
            Func<E, UniTask<TResult>> onFailureAsync)
        {
            if (result.IsSuccess) return onSuccess(result.Value);
            return await onFailureAsync(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, UniTask<TResult>> onSuccessAsync,
            Func<E, TResult> onFailure)
        {
            if (result.IsSuccess) return await onSuccessAsync(result.Value);
            return onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, UniTask<TResult>> onSuccessAsync,
            Func<E, UniTask<TResult>> onFailureAsync)
        {
            if (result.IsSuccess) return await onSuccessAsync(result.Value);
            return await onFailureAsync(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, TResult> onSuccess,
            Func<E, TResult> onFailure)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, TResult> onSuccess,
            Func<E, UniTask<TResult>> onFailureAsync)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? onSuccess(result.Value)
                : await onFailureAsync(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, UniTask<TResult>> onSuccessAsync,
            Func<E, TResult> onFailure)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await onSuccessAsync(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async UniTask<TResult> Match<T, E, TResult>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, UniTask<TResult>> onSuccessAsync,
            Func<E, UniTask<TResult>> onFailureAsync)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? await onSuccessAsync(result.Value)
                : await onFailureAsync(result.Error);
        }
    }
}
#endif

#if NOPE_AWAITABLE
namespace NOPE.Runtime.Core.Result.AwaitableAsync
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, TResult> onSuccess,
            Func<E, Awaitable<TResult>> onFailureAwaitable)
        {
            if (result.IsSuccess)
                return onSuccess(result.Value);
            return await onFailureAwaitable(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, Awaitable<TResult>> onSuccessAwaitable,
            Func<E, TResult> onFailure)
        {
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            return onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Result<T, E> result,
            Func<T, Awaitable<TResult>> onSuccessAwaitable,
            Func<E, Awaitable<TResult>> onFailureAwaitable)
        {
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            return await onFailureAwaitable(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, TResult> onSuccess,
            Func<E, TResult> onFailure)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, TResult> onSuccess,
            Func<E, Awaitable<TResult>> onFailureAwaitable)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return onSuccess(result.Value);
            return await onFailureAwaitable(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, Awaitable<TResult>> onSuccessAwaitable,
            Func<E, TResult> onFailure)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            return onFailure(result.Error);
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static async Awaitable<TResult> Match<T, E, TResult>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, Awaitable<TResult>> onSuccessAwaitable,
            Func<E, Awaitable<TResult>> onFailureAwaitable)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAwaitable(result.Value);
            return await onFailureAwaitable(result.Error);
        }
    }
}
#endif
