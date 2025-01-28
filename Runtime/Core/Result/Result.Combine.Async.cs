using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif



namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {
#if NOPE_UNITASK
        /// <summary>
        /// Combines two asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2), E>> CombineValues<T1, T2, E>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            return (rr1.Value, rr2.Value);
        }

        /// <summary>
        /// Combines three asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3), E>> CombineValues<T1, T2, T3, E>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            return (rr1.Value, rr2.Value, rr3.Value);
        }

        /// <summary>
        /// Combines four asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3, T4), E>> CombineValues<T1, T2, T3, T4, E>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3,
            UniTask<Result<T4, E>> r4)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            return (rr1.Value, rr2.Value, rr3.Value, rr4.Value);
        }

        /// <summary>
        /// Combines five asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3, T4, T5), E>> CombineValues<T1, T2, T3, T4, T5, E>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3,
            UniTask<Result<T4, E>> r4,
            UniTask<Result<T5, E>> r5)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            var rr5 = await r5;
            if (rr5.IsFailure) return rr5.Error;

            return (rr1.Value, rr2.Value, rr3.Value, rr4.Value, rr5.Value);
        }

        /// <summary>
        /// Combines an array of asynchronous results into a single result containing an array of their values.
        /// </summary>
        public static async UniTask<Result<T[], E>> CombineValues<T, E>(
            params UniTask<Result<T, E>>[] results)
        {
            var values = new T[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var r = await results[i];
                if (r.IsFailure)
                    return r.Error;
                values[i] = r.Value;
            }
            return values;
        }


        /// <summary>
        /// Combines multiple asynchronous results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async UniTask<Result<E>> Combine<E>(
            params UniTask<Result<E>>[] results)
        {
            for (int i = 0; i < results.Length; i++)
            {
                var r = await results[i];
                if (r.IsFailure)
                    return r.Error;
            }
            return Result<E>.Success();
        }

        /// <summary>
        /// Combines two asynchronous results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async UniTask<Result<E>> Combine<E, T1, T2>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines three asynchronous results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async UniTask<Result<E>> Combine<E, T1, T2, T3>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines four asynchronous results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async UniTask<Result<E>> Combine<E, T1, T2, T3, T4>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3,
            UniTask<Result<T4, E>> r4)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines five asynchronous results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async UniTask<Result<E>> Combine<E, T1, T2, T3, T4, T5>(
            UniTask<Result<T1, E>> r1,
            UniTask<Result<T2, E>> r2,
            UniTask<Result<T3, E>> r3,
            UniTask<Result<T4, E>> r4,
            UniTask<Result<T5, E>> r5)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            var rr5 = await r5;
            if (rr5.IsFailure) return rr5.Error;

            return Result<E>.Success();
        }
#endif // NOPE_UNITASK

#if NOPE_AWAITABLE
        /// <summary>
        /// Combines two asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2), E>> CombineValues<T1, T2, E>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            return (rr1.Value, rr2.Value);
        }

        /// <summary>
        /// Combines three asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3), E>> CombineValues<T1, T2, T3, E>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            return (rr1.Value, rr2.Value, rr3.Value);
        }

        /// <summary>
        /// Combines four asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3, T4), E>> CombineValues<T1, T2, T3, T4, E>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3,
            Awaitable<Result<T4, E>> r4)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            return (rr1.Value, rr2.Value, rr3.Value, rr4.Value);
        }

        /// <summary>
        /// Combines five asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3, T4, T5), E>> CombineValues<T1, T2, T3, T4, T5, E>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3,
            Awaitable<Result<T4, E>> r4,
            Awaitable<Result<T5, E>> r5)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            var rr5 = await r5;
            if (rr5.IsFailure) return rr5.Error;

            return (rr1.Value, rr2.Value, rr3.Value, rr4.Value, rr5.Value);
        }

        /// <summary>
        /// Combines an array of asynchronous results into a single result containing an array of their values.
        /// </summary>
        public static async Awaitable<Result<T[], E>> CombineValues<T, E>(
            params Awaitable<Result<T, E>>[] results)
        {
            var values = new T[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var r = await results[i];
                if (r.IsFailure)
                    return r.Error;
                values[i] = r.Value;
            }
            return values;
        }


        /// <summary>
        /// Combines multiple asynchronous results into a single result.
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async Awaitable<Result<E>> Combine<E>(
            params Awaitable<Result<E>>[] results)
        {
            for (int i = 0; i < results.Length; i++)
            {
                var r = await results[i];
                if (r.IsFailure)
                    return r.Error;
            }
            return Result<E>.Success();
        }

        /// <summary>
        /// Combines two asynchronous results into a single result.
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async Awaitable<Result<E>> Combine<E, T1, T2>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines three asynchronous results into a single result.
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async Awaitable<Result<E>> Combine<E, T1, T2, T3>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines four asynchronous results into a single result.
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async Awaitable<Result<E>> Combine<E, T1, T2, T3, T4>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3,
            Awaitable<Result<T4, E>> r4)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines five asynchronous results into a single result.
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static async Awaitable<Result<E>> Combine<E, T1, T2, T3, T4, T5>(
            Awaitable<Result<T1, E>> r1,
            Awaitable<Result<T2, E>> r2,
            Awaitable<Result<T3, E>> r3,
            Awaitable<Result<T4, E>> r4,
            Awaitable<Result<T5, E>> r5)
        {
            var rr1 = await r1;
            if (rr1.IsFailure) return rr1.Error;

            var rr2 = await r2;
            if (rr2.IsFailure) return rr2.Error;

            var rr3 = await r3;
            if (rr3.IsFailure) return rr3.Error;

            var rr4 = await r4;
            if (rr4.IsFailure) return rr4.Error;

            var rr5 = await r5;
            if (rr5.IsFailure) return rr5.Error;

            return Result<E>.Success();
        }
#endif // NOPE_AWAITABLE
    }
}
