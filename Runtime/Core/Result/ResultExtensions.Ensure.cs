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
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static Result<T, E> Ensure<T, E>(
            this Result<T, E> result,
            Func<T, bool> predicate,
            E error)
        {
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T, E>.Failure(error);
            }

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
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async UniTask<Result<T, E>> Ensure<T, E>(
            this Result<T, E> result,
            Func<T, UniTask<bool>> asyncPredicate,
            E error)
        {
            if (result.IsSuccess)
            {
                bool valid = await asyncPredicate(result.Value);
                if (!valid)
                    return Result<T, E>.Failure(error);
            }

            return result;
        }

        /// <summary>
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async UniTask<Result<T, E>> Ensure<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, bool> predicate,
            E error)
        {
            var result = await asyncResult;
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T, E>.Failure(error);
            }

            return result;
        }

        /// <summary>
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async UniTask<Result<T, E>> Ensure<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, UniTask<bool>> predicateAsync,
            E error)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                bool valid = await predicateAsync(result.Value);
                if (!valid)
                    return Result<T, E>.Failure(error);
            }

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
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async Awaitable<Result<T, E>> Ensure<T, E>(
            this Result<T, E> result,
            Func<T, Awaitable<bool>> asyncPredicate,
            E error)
        {
            if (result.IsSuccess)
            {
                bool valid = await asyncPredicate(result.Value);
                if (!valid)
                    return Result<T, E>.Failure(error);
            }
            return result;
        }

        /// <summary>
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async Awaitable<Result<T, E>> Ensure<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, bool> predicate,
            E error)
        {
            var result = await asyncResult;
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T, E>.Failure(error);
            }
            return result;
        }

        /// <summary>
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static async Awaitable<Result<T, E>> Ensure<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, Awaitable<bool>> predicateAwaitable,
            E error)
        {
            var result = await asyncResult;
            if (result.IsSuccess)
            {
                bool valid = await predicateAwaitable(result.Value);
                if (!valid)
                    return Result<T, E>.Failure(error);
            }
            return result;
        }
    }
}
#endif
