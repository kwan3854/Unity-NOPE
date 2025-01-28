using System.Linq;

namespace NOPE.Runtime.Core.Result
{
    public readonly partial struct Result
    {
        /// <summary>
        /// Combines two results into a single result containing a tuple of their values.
        /// </summary>
        public static Result<(T1, T2), E> CombineValues<T1, T2, E>(
            Result<T1, E> r1,
            Result<T2, E> r2)
        {
            if (r1.IsFailure) return Result<(T1, T2), E>.Failure(r1.Error);
            if (r2.IsFailure) return Result<(T1, T2), E>.Failure(r2.Error);
            return (r1.Value, r2.Value);
        }

        /// <summary>
        /// Combines three results into a single result containing a tuple of their values.
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
        /// Combines four results into a single result containing a tuple of their values.
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
        /// Combines five results into a single result containing a tuple of their values.
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
        /// Combines an array of results into a single result containing an array of their values.
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
        /// Combines multiple results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static Result<E> Combine<E>(params Result<E>[] results)
        {
            var firstFail = results.FirstOrDefault(r => r.IsFailure);
            if (firstFail.IsFailure)
                return Result<E>.Failure(firstFail.Error);

            return Result<E>.Success();
        }

        /// <summary>
        /// Combines two results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static Result<E> Combine<E, T1, T2>(
            Result<T1, E> r1,
            Result<T2, E> r2)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            return Result<E>.Success();
        }

        /// <summary>
        /// Combines three results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static Result<E> Combine<E, T1, T2, T3>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            return Result<E>.Success();
        }

        /// <summary>
        /// Combines four results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static Result<E> Combine<E, T1, T2, T3, T4>(
            Result<T1, E> r1,
            Result<T2, E> r2,
            Result<T3, E> r3,
            Result<T4, E> r4)
        {
            if (r1.IsFailure) return r1.Error;
            if (r2.IsFailure) return r2.Error;
            if (r3.IsFailure) return r3.Error;
            if (r4.IsFailure) return r4.Error;
            return Result<E>.Success();
        }

        /// <summary>
        /// Combines five results into a single result. 
        /// Returns the first failure if any, otherwise returns success.
        /// </summary>
        public static Result<E> Combine<E, T1, T2, T3, T4, T5>(
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
            return Result<E>.Success();
        }
    }
}
