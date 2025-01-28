using System.Linq;
using UnityEngine; // for debug logs if needed

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

#if NOPE_AWAITABLE
// using NOPE.Runtime.Core.Awaitable;
#endif

namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {

        /// <summary>
        /// Combines multiple Results into a single Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<(T1, T2), E> CombineValues<T1, T2, E>(
            Result<T1, E> r1,
            Result<T2, E> r2)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            return (r1.Value, r2.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<(T1, T2, T3), E> CombineValues<T1, T2, T3, E>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            return (r1.Value, r2.Value, r3.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<(T1, T2, T3, T4), E> CombineValues<T1, T2, T3, T4, E>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3,
            Result<T4, E> r4)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            return (r1.Value, r2.Value, r3.Value, r4.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<(T1, T2, T3, T4, T5), E> CombineValues<T1, T2, T3, T4, T5, E>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3,
            Result<T4, E> r4,
            Result<T5, E> r5)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            if (r5.IsFailure) return r5.Error;
            return (r1.Value, r2.Value, r3.Value, r4.Value, r5.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<T[], E> CombineValues<T, E>(
            params Result<T, E>[] results)
        {
            var firstFail = results.FirstOrDefault(r => r.IsFailure);
            if (firstFail.IsFailure)
                return firstFail.Error;

            var values = results.Select(r => r.Value).ToArray();
            return values;
        }

        /// <summary>
        /// Combines multiple Results into a single Unit Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<Unit, E> Combine<E>(params Result<Unit, E>[] results)
        {
            var firstFail = results.FirstOrDefault(r => r.IsFailure);
            if (firstFail.IsFailure)
                return Result<Unit, E>.Failure(firstFail.Error);

            return Result<Unit, E>.Success(Unit.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Unit Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<Unit, E> Combine<E, T1, T2>(
            Result<T1, E> r1,
            Result<T2, E> r2)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            return Result<Unit,E>.Success(Unit.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Unit Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<Unit, E> Combine<E, T1, T2, T3>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            return Result<Unit,E>.Success(Unit.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Unit Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<Unit, E> Combine<E, T1, T2, T3, T4>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3,
            Result<T4, E> r4)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            return Result<Unit,E>.Success(Unit.Value);
        }

        /// <summary>
        /// Combines multiple Results into a single Unit Result. If any of the input Results are failures, the first failure is returned.
        /// </summary>
        public static Result<Unit, E> Combine<E, T1, T2, T3, T4, T5>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3,
            Result<T4, E> r4,
            Result<T5, E> r5)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            if (r5.IsFailure) return r5.Error;
            return Result<Unit,E>.Success(Unit.Value);
        }
    }
}
