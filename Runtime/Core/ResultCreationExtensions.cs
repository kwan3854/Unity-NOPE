using System;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// Provides utility methods for creating Result&lt;T&gt; instances
    /// in a more convenient or condition-based manner.
    /// </summary>
    public static partial class Result
    {
        /// <summary>
        /// Returns a Success(resultValue) if <paramref name="condition"/> is true;
        /// otherwise returns Failure(errorMessage).
        /// </summary>
        public static Result<T> SuccessIf<T>(
            bool condition,
            T resultValue,
            string errorMessage)
        {
            return condition
                ? Result<T>.Success(resultValue)
                : Result<T>.Failure(errorMessage);
        }

        /// <summary>
        /// Returns a Success(resultValue) if <paramref name="condition"/> is true;
        /// otherwise returns Failure(errorMessage).
        /// </summary>
        /// <remarks>
        /// Overload that uses a Func&lt;bool&gt; for lazy evaluation of the condition.
        /// </remarks>
        public static Result<T> SuccessIf<T>(
            Func<bool> condition,
            T resultValue,
            string errorMessage)
        {
            return condition()
                ? Result<T>.Success(resultValue)
                : Result<T>.Failure(errorMessage);
        }

        /// <summary>
        /// Returns a Failure(errorValue) if <paramref name="condition"/> is true;
        /// otherwise returns Success(successValue).
        /// </summary>
        public static Result<T> FailureIf<T>(
            bool condition,
            T successValue,
            string errorMessage)
        {
            return condition
                ? Result<T>.Failure(errorMessage)
                : Result<T>.Success(successValue);
        }

        /// <summary>
        /// Returns a Failure(errorValue) if <paramref name="condition"/> is true;
        /// otherwise returns Success(successValue).
        /// </summary>
        public static Result<T> FailureIf<T>(
            Func<bool> condition,
            T successValue,
            string errorMessage)
        {
            return condition()
                ? Result<T>.Failure(errorMessage)
                : Result<T>.Success(successValue);
        }

        /// <summary>
        /// Wraps a function call that may throw an exception and returns:
        /// - Success(result) if no exception
        /// - Failure(exception.Message) if an exception is thrown.
        /// You can customize error handling by passing a custom converter function.
        /// </summary>
        public static Result<T> Of<T>(
            Func<T> func,
            Func<Exception, string> errorConverter = null)
        {
            try
            {
                T value = func();
                return Result<T>.Success(value);
            }
            catch (Exception ex)
            {
                if (errorConverter != null)
                    return Result<T>.Failure(errorConverter(ex));
                return Result<T>.Failure(ex.Message);
            }
        }
    }
}