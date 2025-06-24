using NOPE.Runtime.Core.Result;
using UnityEngine;
#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Converts this Maybe to a Result, returning Success if it has a value,
        /// or Failure with the provided error if it does not.
        /// </summary>
        public static Result<T, E> ToResult<T, E>(
            this Maybe<T> maybe,
            E error)
        {
            return maybe.HasValue
                ? Result<T, E>.Success(maybe.Value)
                : Result<T, E>.Failure(error);
        }

#if NOPE_UNITASK
        /// <summary>
        /// Converts an asynchronous Maybe (UniTask<Maybe<T>>) to a Result.
        /// It awaits the Maybe and then converts it to a Result.
        /// </summary>
        public static async UniTask<Result<T, E>> ToResult<T, E>(
            this UniTask<Maybe<T>> asyncMaybe,
            E error)
        {
            var maybe = await asyncMaybe;
            return maybe.ToResult(error);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Converts an asynchronous Maybe (Awaitable<Maybe<T>>) to a Result.
        /// It awaits the Maybe and then converts it to a Result.
        /// </summary>
        public static async Awaitable<Result<T, E>> ToResult<T, E>(
            this Awaitable<Maybe<T>> asyncMaybe,
            E error)
        {
            var maybe = await asyncMaybe;
            return maybe.ToResult(error);
        }
#endif
    }
}