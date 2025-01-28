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
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static Result<T, E> Tap<T, E>(
            this Result<T, E> result,
            Action<T> action)
        {
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Result.UniTaskAsync
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async UniTask<Result<T, E>> Tap<T, E>(
            this Result<T, E> result,
            Func<T, UniTask> asyncAction)
        {
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }

        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async UniTask<Result<T, E>> Tap<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Action<T> action)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }

        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async UniTask<Result<T, E>> Tap<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, UniTask> asyncAction)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
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
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async Awaitable<Result<T, E>> Tap<T, E>(
            this Result<T, E> result,
            Func<T, Awaitable> asyncAction)
        {
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }

        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async Awaitable<Result<T, E>> Tap<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Action<T> action)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                action(result.Value);
            return result;
        }

        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (side-effect only)
        /// </summary>
        public static async Awaitable<Result<T, E>> Tap<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, Awaitable> asyncAction)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
                await asyncAction(result.Value);
            return result;
        }
    }
}
#endif