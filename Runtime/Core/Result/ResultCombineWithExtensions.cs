using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {
        /// <summary>
        /// Combines two results into a single result containing a tuple of their values.
        /// </summary>
        public static Result<(T1, T2)> CombineWith<T1, T2>(Result<T1> r1, Result<T2> r2)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            return Result<(T1, T2)>.Success((r1.Value, r2.Value));
        }

        /// <summary>
        /// Combines three results into a single result containing a tuple of their values.
        /// </summary>
        public static Result<(T1, T2, T3)> CombineWith<T1, T2, T3>(Result<T1> r1, Result<T2> r2, Result<T3> r3)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            return Result<(T1, T2, T3)>.Success((r1.Value, r2.Value, r3.Value));
        }

        /// <summary>
        /// Combines four results into a single result containing a tuple of their values.
        /// </summary>
        public static Result<(T1, T2, T3, T4)> CombineWith<T1, T2, T3, T4>(Result<T1> r1, Result<T2> r2, Result<T3> r3, Result<T4> r4)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            return Result<(T1, T2, T3, T4)>.Success((r1.Value, r2.Value, r3.Value, r4.Value));
        }

        /// <summary>
        /// Combines five results into a single result containing a tuple of their values.
        /// </summary>
        public static Result<(T1, T2, T3, T4, T5)> CombineWith<T1, T2, T3, T4, T5>(Result<T1> r1, Result<T2> r2, Result<T3> r3, Result<T4> r4, Result<T5> r5)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            if (r5.IsFailure) return r5.Error;
            return Result<(T1, T2, T3, T4, T5)>.Success((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value));
        }
        
        /// <summary>
        /// Combines an array of awaitable results into a single result.
        /// </summary>
        public static Result<T[]> CombineWith<T>(params Result<T>[] results)
        {
            var failures = results.Where(r => r.IsFailure).ToList();
            if (failures.Any())
                return failures.First().Error;

            var values = results.Select(r => r.Value).ToArray();
            return Result<T[]>.Success(values);
        }

#if NOPE_UNITASK
        /// <summary>
        /// Combines two asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2)>> CombineWith<T1, T2>(UniTask<Result<T1>> r1, UniTask<Result<T2>> r2)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            return Result<(T1, T2)>.Success((result1.Value, result2.Value));
        }

        /// <summary>
        /// Combines three asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3)>> CombineWith<T1, T2, T3>(UniTask<Result<T1>> r1, UniTask<Result<T2>> r2, UniTask<Result<T3>> r3)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            return Result<(T1, T2, T3)>.Success((result1.Value, result2.Value, result3.Value));
        }

        /// <summary>
        /// Combines four asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3, T4)>> CombineWith<T1, T2, T3, T4>(UniTask<Result<T1>> r1, UniTask<Result<T2>> r2, UniTask<Result<T3>> r3, UniTask<Result<T4>> r4)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            return Result<(T1, T2, T3, T4)>.Success((result1.Value, result2.Value, result3.Value, result4.Value));
        }

        /// <summary>
        /// Combines five asynchronous results into a single result containing a tuple of their values.
        /// </summary>
        public static async UniTask<Result<(T1, T2, T3, T4, T5)>> CombineWith<T1, T2, T3, T4, T5>(UniTask<Result<T1>> r1, UniTask<Result<T2>> r2, UniTask<Result<T3>> r3, UniTask<Result<T4>> r4, UniTask<Result<T5>> r5)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            var result5 = await r5;
            if (result5.IsFailure) return result5.Error;

            return Result<(T1, T2, T3, T4, T5)>.Success((result1.Value, result2.Value, result3.Value, result4.Value, result5.Value));
        }

        /// <summary>
        /// Combines an array of asynchronous results into a single result.
        /// </summary>
        public static async UniTask<Result<T[]>> CombineWith<T>(params UniTask<Result<T>>[] results)
        {
            var values = new T[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var result = await results[i];
                if (result.IsFailure)
                    return result.Error;
                values[i] = result.Value;
            }

            return Result<T[]>.Success(values);
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Combines two awaitable results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2)>> CombineWith<T1, T2>(Awaitable<Result<T1>> r1, Awaitable<Result<T2>> r2)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            return Result<(T1, T2)>.Success((result1.Value, result2.Value));
        }

        /// <summary>
        /// Combines three awaitable results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3)>> CombineWith<T1, T2, T3>(Awaitable<Result<T1>> r1, Awaitable<Result<T2>> r2, Awaitable<Result<T3>> r3)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            return Result<(T1, T2, T3)>.Success((result1.Value, result2.Value, result3.Value));
        }

        /// <summary>
        /// Combines four awaitable results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3, T4)>> CombineWith<T1, T2, T3, T4>(Awaitable<Result<T1>> r1, Awaitable<Result<T2>> r2, Awaitable<Result<T3>> r3, Awaitable<Result<T4>> r4)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            return Result<(T1, T2, T3, T4)>.Success((result1.Value, result2.Value, result3.Value, result4.Value));
        }

        /// <summary>
        /// Combines five awaitable results into a single result containing a tuple of their values.
        /// </summary>
        public static async Awaitable<Result<(T1, T2, T3, T4, T5)>> CombineWith<T1, T2, T3, T4, T5>(Awaitable<Result<T1>> r1, Awaitable<Result<T2>> r2, Awaitable<Result<T3>> r3, Awaitable<Result<T4>> r4, Awaitable<Result<T5>> r5)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            var result5 = await r5;
            if (result5.IsFailure) return result5.Error;

            return Result<(T1, T2, T3, T4, T5)>.Success((result1.Value, result2.Value, result3.Value, result4.Value, result5.Value));
        }
        
        /// <summary>
        /// Combines an array of awaitable results into a single result.
        /// </summary>
        public static async Awaitable<Result<T[]>> CombineWith<T>(params Awaitable<Result<T>>[] results)
        {
            var values = new T[results.Length];
            for (int i = 0; i < results.Length; i++)
            {
                var result = await results[i];
                if (result.IsFailure)
                    return result.Error;
                values[i] = result.Value;
            }

            return Result<T[]>.Success(values);
        }
#endif
    }
}