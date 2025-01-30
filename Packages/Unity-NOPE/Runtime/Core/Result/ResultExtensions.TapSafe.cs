using System;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        public static Result<T, E> TapSafe<T, E>(
            this Result<T, E> result,
            Action<T> action,
            Func<Exception, E> errorHandler)

        {
            try
            {
                if (result.IsSuccess)
                    action(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }
        
#if NOPE_UNITASK
        public static async UniTask<Result<T, E>> TapSafe<T, E>(
            this Result<T, E> result,
            Func<T, UniTask> asyncAction,
            Func<Exception, E> errorHandler)
        {
            try
            {
                if (result.IsSuccess)
                    await asyncAction(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }

        public static async UniTask<Result<T, E>> TapSafe<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Action<T> action,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            try
            {
                if (result.IsSuccess)
                    action(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }

        public static async UniTask<Result<T, E>> TapSafe<T, E>(
            this UniTask<Result<T, E>> asyncResult,
            Func<T, UniTask> asyncAction,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            try
            {
                if (result.IsSuccess)
                    await asyncAction(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }
#endif
        
#if NOPE_AWAITABLE
        public static async Awaitable<Result<T, E>> TapSafe<T, E>(
            this Result<T, E> result,
            Func<T, Awaitable> asyncAction,
            Func<Exception, E> errorHandler)
        {
            try
            {
                if (result.IsSuccess)
                    await asyncAction(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }

        public static async Awaitable<Result<T, E>> TapSafe<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Action<T> action,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            try
            {
                if (result.IsSuccess)
                    action(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }

        public static async Awaitable<Result<T, E>> TapSafe<T, E>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<T, Awaitable> asyncAction,
            Func<Exception, E> errorHandler)
        {
            var result = await asyncResult;
            try
            {
                if (result.IsSuccess)
                    await asyncAction(result.Value);
                return result;
            }
            catch (Exception e)
            {
                return Result<T, E>.Failure(errorHandler(e));
            }
        }
#endif
    }
}
