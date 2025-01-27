using System;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// Provides extension methods for Result&lt;T&gt; to enable functional-style chaining.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Projects the value of a successful Result into a new form.
        /// If the Result is failure, returns a new Result with the same error.
        /// </summary>
        public static Result<TNew> Map<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, TNew> selector)
        {
            return result.IsSuccess
                ? Result<TNew>.Success(selector(result.Value))
                : Result<TNew>.Failure(result.Error);
        }

        /// <summary>
        /// Converts the value of a successful Result into another Result.
        /// (aka 'flatMap' or 'SelectMany')
        /// If the current Result is failure, returns a new Result with the same error.
        /// </summary>
        public static Result<TNew> Bind<TOriginal, TNew>(
            this Result<TOriginal> result,
            Func<TOriginal, Result<TNew>> binder)
        {
            return result.IsSuccess
                ? binder(result.Value)
                : Result<TNew>.Failure(result.Error);
        }

        /// <summary>
        /// Executes the given action if the Result is successful, then returns the original Result as-is.
        /// (Useful for "do something" side-effects without changing the type)
        /// </summary>
        public static Result<T> Tap<T>(
            this Result<T> result,
            Action<T> action)
        {
            if (result.IsSuccess)
            {
                action(result.Value);
            }
            return result;
        }

        /// <summary>
        /// If this Result is successful but fails the predicate, transforms it into a failure with the given error message.
        /// Otherwise, returns the original Result.
        /// </summary>
        public static Result<T> Ensure<T>(
            this Result<T> result,
            Func<T, bool> predicate,
            string errorMessage)
        {
            if (result.IsSuccess && !predicate(result.Value))
            {
                return Result<T>.Failure(errorMessage);
            }
            return result;
        }

        /// <summary>
        /// Transforms a Result into a single value by providing two functions:
        /// one for handling success, and one for handling failure.
        /// </summary>
        public static TResult Match<T, TResult>(
            this Result<T> result,
            Func<T, TResult> onSuccess,
            Func<string, TResult> onFailure)
        {
            return result.IsSuccess
                ? onSuccess(result.Value)
                : onFailure(result.Error);
        }

        /// <summary>
        /// If the Result is failure, applies a function to the error message to produce a new error.
        /// Otherwise, returns the original success.
        /// </summary>
        public static Result<T> MapError<T>(
            this Result<T> result,
            Func<string, string> errorSelector)
        {
            if (result.IsFailure)
            {
                var transformed = errorSelector(result.Error);
                return Result<T>.Failure(transformed);
            }
            return result;
        }
    }
}