using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// Async extension methods for Result&lt;T&gt; supporting UniTask or Unity's Awaitable.
    /// Depending on define constraints, these methods become available.
    /// </summary>
    public static class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods -------------------

        /// <summary>
        /// (UniTask) Asynchronously maps the value of a successful Result.
        /// If the Result is failure, returns the same error.
        /// </summary>
        public static async UniTask<Result<TNew>> Map<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result<TNew>.Success(selector(result.Value))
                : Result<TNew>.Failure(result.Error);
        }

        /// <summary>
        /// (UniTask) Asynchronously binds the value of a successful Result to a new Result.
        /// If the Result is failure, returns the same error.
        /// </summary>
        public static async UniTask<Result<TNew>> Bind<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<Result<TNew>>> binder)
        {
            var result = await asyncResult;
            if (!result.IsSuccess)
                return Result<TNew>.Failure(result.Error);

            return await binder(result.Value);
        }

        /// <summary>
        /// (UniTask) Asynchronously executes a side effect if the Result is successful.
        /// Returns the original Result.
        /// </summary>
        public static async UniTask<Result<T>> Tap<T>(
            this UniTask<Result<T>> asyncResult,
            Func<T, UniTask> sideEffect)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                await sideEffect(result.Value);
            }
            return result;
        }

        /// <summary>
        /// (UniTask) Asynchronously ensures the Result meets a condition.
        /// If the condition fails, returns a failure with the given error message.
        /// </summary>
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

        /// <summary>
        /// (UniTask) Asynchronously matches the Result to a value based on success or failure.
        /// </summary>
        public static async UniTask<TNew> Match<TOriginal, TNew>(
            this UniTask<Result<TOriginal>> asyncResult,
            Func<TOriginal, UniTask<TNew>> onSuccessAsync,
            Func<string, UniTask<TNew>> onFailureAsync)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                return await onSuccessAsync(result.Value);
            else
                return await onFailureAsync(result.Error);
        }
#endif // NOPE_UNITASK


#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods -------------------

        /// <summary>
        /// (Awaitable) Asynchronously maps the value of a successful Result.
        /// If the Result is failure, returns the same error.
        /// </summary>
        public static async Awaitable<Result<TNew>> Map<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult; 
            if (result.IsSuccess)
                return Result<TNew>.Success(selector(result.Value));
            return Result<TNew>.Failure(result.Error);
        }

        /// <summary>
        /// (Awaitable) Asynchronously binds the value of a successful Result to a new Result.
        /// If the Result is failure, returns the same error.
        /// </summary>
        public static async Awaitable<Result<TNew>> Bind<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<Result<TNew>>> binder)
        {
            var result = await asyncResult;
            if (!result.IsSuccess)
                return Result<TNew>.Failure(result.Error);

            return await binder(result.Value);
        }

        /// <summary>
        /// (Awaitable) Asynchronously executes a side effect if the Result is successful.
        /// Returns the original Result.
        /// </summary>
        public static async Awaitable<Result<T>> Tap<T>(
            this Awaitable<Result<T>> asyncResult,
            Func<T, Awaitable> sideEffect)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                await sideEffect(result.Value);
            }
            return result;
        }

        /// <summary>
        /// (Awaitable) Asynchronously ensures the Result meets a condition.
        /// If the condition fails, returns a failure with the given error message.
        /// </summary>
        public static async Awaitable<Result<T>> Ensure<T>(
            this Awaitable<Result<T>> asyncResult,
            Func<T, Awaitable<bool>> predicateAwaitable,
            string errorMessage)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                bool valid = await predicateAwaitable(result.Value);
                if (!valid)
                    return Result<T>.Failure(errorMessage);
            }
            return result;
        }

        /// <summary>
        /// (Awaitable) Asynchronously matches the Result to a value based on success or failure.
        /// </summary>
        public static async Awaitable<TNew> Match<TOriginal, TNew>(
            this Awaitable<Result<TOriginal>> asyncResult,
            Func<TOriginal, Awaitable<TNew>> onSuccessAwaitable,
            Func<string, Awaitable<TNew>> onFailureAwaitable)
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