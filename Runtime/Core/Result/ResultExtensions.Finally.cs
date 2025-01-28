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
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static TOut Finally<T, E, TOut>(
            this Result<T, E> result,
            Func<Result<T, E>, TOut> finalFunc)
        {
            return finalFunc(result);
        }

#if NOPE_UNITASK
        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async UniTask<TOut> Finally<T, E, TOut>(
            this Result<T, E> result,
            Func<Result<T, E>, UniTask<TOut>> finalFunc)
        {
            return await finalFunc(result);
        }
        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async UniTask<TOut> Finally<T, E, TOut>(
            this UniTask<Result<T, E>> asyncResult,
            Func<Result<T, E>, TOut> finalFunc)
        {
            var result = await asyncResult;
            return finalFunc(result);
        }

        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async UniTask<TOut> Finally<T, E, TOut>(
            this UniTask<Result<T, E>> asyncResult,
            Func<Result<T, E>, UniTask<TOut>> finalFunc)
        {
            var result = await asyncResult;
            return await finalFunc(result);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, E, TOut>(
            this Result<T, E> result,
            Func<Result<T, E>, Awaitable<TOut>> finalFunc)
        {
            return await finalFunc(result);
        }

        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, E, TOut>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<Result<T, E>, TOut> finalFunc)
        {
            var result = await asyncResult;
            return finalFunc(result);
        }

        /// <summary>
        /// Executes a final function regardless of success or failure,
        /// returning whatever finalFunc produces.
        /// </summary>
        public static async Awaitable<TOut> FinallyAwaitable<T, E, TOut>(
            this Awaitable<Result<T, E>> asyncResult,
            Func<Result<T, E>, Awaitable<TOut>> finalFunc)
        {
            var result = await asyncResult;
            return await finalFunc(result);
        }
#endif
        
    }
}
