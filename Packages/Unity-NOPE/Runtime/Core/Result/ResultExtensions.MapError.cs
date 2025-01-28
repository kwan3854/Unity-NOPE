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
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static Result<T, E2> MapError<T, E1, E2>(
            this Result<T, E1> result,
            Func<E1, E2> errorSelector)
        {
            if (result.IsFailure)
            {
                var newErr = errorSelector(result.Error);
                return Result<T, E2>.Failure(newErr);
            }
            // Success
            return Result<T, E2>.Success(result.Value);
        }

#if NOPE_UNITASK
        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async UniTask<Result<T, E2>> MapError<T, E1, E2>(
            this Result<T, E1> result,
            Func<E1, UniTask<E2>> errorSelectorAsync)
        {
            if (result.IsFailure)
            {
                var newErr = await errorSelectorAsync(result.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(result.Value);
        }

        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async UniTask<Result<T, E2>> MapError<T, E1, E2>(
            this UniTask<Result<T, E1>> asyncResult,
            Func<E1, E2> errorSelector)
        {
            var r = await asyncResult;
            if (r.IsFailure)
            {
                var newErr = errorSelector(r.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(r.Value);
        }

        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async UniTask<Result<T, E2>> MapError<T, E1, E2>(
            this UniTask<Result<T, E1>> asyncResult,
            Func<E1, UniTask<E2>> errorSelectorAsync)
        {
            var r = await asyncResult;
            if (r.IsFailure)
            {
                var newErr = await errorSelectorAsync(r.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(r.Value);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async Awaitable<Result<T, E2>> MapError<T, E1, E2>(
            this Result<T, E1> result,
            Func<E1, Awaitable<E2>> errorSelectorAwaitable)
        {
            if (result.IsFailure)
            {
                var newErr = await errorSelectorAwaitable(result.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(result.Value);
        }

        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async Awaitable<Result<T, E2>> MapError<T, E1, E2>(
            this Awaitable<Result<T, E1>> asyncResult,
            Func<E1, E2> errorSelector)
        {
            var r = await asyncResult;
            if (r.IsFailure)
            {
                var newErr = errorSelector(r.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(r.Value);
        }

        /// <summary>
        /// If failure => transform the error E1->E2. If success => keep Value same, return Result&lt;T,E2&gt;.Success.
        /// </summary>
        public static async Awaitable<Result<T, E2>> MapError<T, E1, E2>(
            this Awaitable<Result<T, E1>> asyncResult,
            Func<E1, Awaitable<E2>> errorSelectorAwaitable)
        {
            var r = await asyncResult;
            if (r.IsFailure)
            {
                var newErr = await errorSelectorAwaitable(r.Error);
                return Result<T, E2>.Failure(newErr);
            }
            return Result<T, E2>.Success(r.Value);
        }
#endif // NOPE_AWAITABLE
    }
}
