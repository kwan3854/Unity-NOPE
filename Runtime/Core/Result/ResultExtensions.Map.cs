using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif


namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static Result<TNew, E> Map<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, TNew> selector)
        {
            return result.IsSuccess
                ? Result<TNew, E>.Success(selector(result.Value))
                : Result<TNew, E>.Failure(result.Error);
        }
    }
}

#if NOPE_UNITASK
namespace NOPE.Runtime.Core.Result.UniTaskAsync
{
    public static partial class ResultExtensions
    {
        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async UniTask<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, UniTask<TNew>> asyncSelector)
        {
            if (result.IsFailure)
                return result.Error;

            var newVal = await asyncSelector(result.Value);
            return newVal;
        }

        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async UniTask<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? selector(result.Value)
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async UniTask<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this UniTask<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, UniTask<TNew>> asyncSelector)
        {
            var result = await asyncResult;
            if (result.IsFailure)
                return result.Error;

            var newVal = await asyncSelector(result.Value);
            return newVal;
        }
    }
}
#endif

#if NOPE_AWAITABLE
namespace NOPE.Runtime.Core.Result.AwaitableAsync
{
    public static partial class ResultExtensions
    {
        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async Awaitable<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this Result<TOriginal, E> result,
            Func<TOriginal, Awaitable<TNew>> asyncSelector)
        {
            if (result.IsFailure) 
                return Result<TNew, E>.Failure(result.Error);

            var newVal = await asyncSelector(result.Value);
            return Result<TNew, E>.Success(newVal);
        }

        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async Awaitable<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, TNew> selector)
        {
            var result = await asyncResult;
            return result.IsSuccess
                ? Result<TNew, E>.Success(selector(result.Value))
                : Result<TNew, E>.Failure(result.Error);
        }

        /// <summary>Maps the value of a successful Result into a new form.</summary>
        public static async Awaitable<Result<TNew, E>> Map<TOriginal, TNew, E>(
            this Awaitable<Result<TOriginal, E>> asyncResult,
            Func<TOriginal, Awaitable<TNew>> asyncSelector)
        {
            var result = await asyncResult;
            if (result.IsFailure)
                return result.Error;

            var newVal = await asyncSelector(result.Value);
            return newVal;
        }
    }
}
#endif
