using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result.Async
{
    public static partial class ResultAsyncExtensions
    {
#if NOPE_UNITASK
        // ------------------- UniTask-based Async Methods (FINALLY, CFE Style) -------------------
        
        // /// <summary>
        // /// (1) Sync Result<T> => Sync finalFunc => returns TOut (end of chain)
        // /// </summary>
        // public static TOut Finally<T, TOut>(
        //     this Result<T> result,
        //     Func<Result<T>, TOut> finalFunc)
        // {
        //     return finalFunc(result);
        // }

        /// <summary>
        /// (2) Sync Result<T> => Async finalFunc => returns UniTask<TOut> (end of chain)
        /// </summary>
        public static async UniTask<TOut> Finally<T, TOut>(
            this Result<T> result,
            Func<Result<T>, UniTask<TOut>> finalFunc)
        {
            return await finalFunc(result);
        }

        /// <summary>
        /// (3) Async UniTask<Result<T>> => Sync finalFunc => returns UniTask<TOut> (end of chain)
        /// </summary>
        public static async UniTask<TOut> Finally<T, TOut>(
            this UniTask<Result<T>> asyncResult,
            Func<Result<T>, TOut> finalFunc)
        {
            var result = await asyncResult;
            return finalFunc(result);
        }

        /// <summary>
        /// (4) Async UniTask<Result<T>> => Async finalFunc => returns UniTask<TOut> (end of chain)
        /// </summary>
        public static async UniTask<TOut> Finally<T, TOut>(
            this UniTask<Result<T>> asyncResult,
            Func<Result<T>, UniTask<TOut>> finalFunc)
        {
            var result = await asyncResult;
            return await finalFunc(result);
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        // ------------------- Awaitable-based Async Methods (FINALLY, CFE Style) -------------------

        // /// <summary>
        // /// (1) Sync Result<T> => Sync finalFunc => returns TOut (end of chain)
        // /// </summary>
        // public static TOut Finally<T, TOut>(
        //     this Result<T> result,
        //     Func<Result<T>, TOut> finalFunc)
        // {
        //     return finalFunc(result);
        // }

        /// <summary>
        /// (2) Sync Result<T> => Async finalFunc => returns Awaitable<TOut> (end of chain)
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, TOut>(
            this Result<T> result,
            Func<Result<T>, Awaitable<TOut>> finalFunc)
        {
            return await finalFunc(result);
        }

        /// <summary>
        /// (3) Async Awaitable<Result<T>> => Sync finalFunc => returns Awaitable<TOut> (end of chain)
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, TOut>(
            this Awaitable<Result<T>> asyncResult,
            Func<Result<T>, TOut> finalFunc)
        {
            var result = await asyncResult;
            return finalFunc(result);
        }

        /// <summary>
        /// (4) Async Awaitable<Result<T>> => Async finalFunc => returns Awaitable<TOut> (end of chain)
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, TOut>(
            this Awaitable<Result<T>> asyncResult,
            Func<Result<T>, Awaitable<TOut>> finalFunc)
        {
            var result = await asyncResult;
            return await finalFunc(result);
        }
#endif
    }
}