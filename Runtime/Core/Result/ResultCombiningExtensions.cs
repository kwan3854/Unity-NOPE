using System.Linq;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {
        public static Result Combine(params Result<object>[] results)
        {
            var failures = results.Where(r => r.IsFailure).ToList();
            if (failures.Any())
                return failures.First().Error;

            return Success();
        }

        public static Result Combine<T1, T2>(Result<T1> r1, Result<T2> r2)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            
            return Success();
        }

        public static Result Combine<T1, T2, T3>(Result<T1> r1, Result<T2> r2, Result<T3> r3)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            
            return Success();
        }

        public static Result Combine<T1, T2, T3, T4>(Result<T1> r1, Result<T2> r2, Result<T3> r3, Result<T4> r4)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            
            return Success();
        }

        public static Result Combine<T1, T2, T3, T4, T5>(Result<T1> r1, Result<T2> r2, Result<T3> r3, Result<T4> r4, Result<T5> r5)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            if (r5.IsFailure) return r5.Error;
            
            return Success();
        }

#if NOPE_UNITASK
        public static async UniTask<Result> Combine(params UniTask<Result<object>>[] results)
        {
            foreach (var asyncResult in results)
            {
                var result = await asyncResult;
                if (result.IsFailure)
                    return result.Error;
            }
            return Success();
        }
        
        public static async UniTask<Result> Combine<T1, T2>(
            UniTask<Result<T1>> r1, 
            UniTask<Result<T2>> r2)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;
        
            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;
        
            return Success();
        }
        
        public static async UniTask<Result> Combine<T1, T2, T3>(
            UniTask<Result<T1>> r1, 
            UniTask<Result<T2>> r2, 
            UniTask<Result<T3>> r3)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            return Success();
        }

        public static async UniTask<Result> Combine<T1, T2, T3, T4>(
            UniTask<Result<T1>> r1, 
            UniTask<Result<T2>> r2, 
            UniTask<Result<T3>> r3, 
            UniTask<Result<T4>> r4)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            return Success();
        }

        public static async UniTask<Result> Combine<T1, T2, T3, T4, T5>(
            UniTask<Result<T1>> r1, 
            UniTask<Result<T2>> r2, 
            UniTask<Result<T3>> r3, 
            UniTask<Result<T4>> r4, 
            UniTask<Result<T5>> r5)
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

            return Success();
        }
#endif

#if NOPE_AWAITABLE
        public static async Awaitable<Result> Combine(params Awaitable<Result<object>>[] results)
        {
            foreach (var asyncResult in results)
            {
                var result = await asyncResult;
                if (result.IsFailure)
                    return result.Error;
            }
            return Success();
        }
        
        public static async Awaitable<Result> Combine<T1, T2>(
            Awaitable<Result<T1>> r1, 
            Awaitable<Result<T2>> r2)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;
        
            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;
        
            return Success();
        }
        
        public static async Awaitable<Result> Combine<T1, T2, T3>(
            Awaitable<Result<T1>> r1, 
            Awaitable<Result<T2>> r2, 
            Awaitable<Result<T3>> r3)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            return Success();
        }

        public static async Awaitable<Result> Combine<T1, T2, T3, T4>(
            Awaitable<Result<T1>> r1, 
            Awaitable<Result<T2>> r2, 
            Awaitable<Result<T3>> r3, 
            Awaitable<Result<T4>> r4)
        {
            var result1 = await r1;
            if (result1.IsFailure) return result1.Error;

            var result2 = await r2;
            if (result2.IsFailure) return result2.Error;

            var result3 = await r3;
            if (result3.IsFailure) return result3.Error;

            var result4 = await r4;
            if (result4.IsFailure) return result4.Error;

            return Success();
        }

        public static async Awaitable<Result> Combine<T1, T2, T3, T4, T5>(
            Awaitable<Result<T1>> r1, 
            Awaitable<Result<T2>> r2, 
            Awaitable<Result<T3>> r3, 
            Awaitable<Result<T4>> r4, 
            Awaitable<Result<T5>> r5)
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

            return Success();
        }
#endif
    }
}