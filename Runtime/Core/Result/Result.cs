using System;

namespace NOPE.Runtime.Core.Result
{
    /// <summary>
    /// Represents the outcome of an operation that can either succeed (with a value T)
    /// or fail (with an error E).
    /// </summary>
    public readonly struct Result<T, E>
    {
        private readonly T _value;
        private readonly E _error;

        private Result(bool isSuccess, T value, E error)
        {
            IsSuccess = isSuccess;
            _value = value;
            _error = error;
        }

        /// <summary>Whether this result represents a successful outcome.</summary>
        public bool IsSuccess { get; }

        /// <summary>Whether this result represents a failed outcome.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The value of the result if it is successful; otherwise throws an exception.
        /// </summary>
        public T Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Attempted to access Value of a failed Result.");

        /// <summary>
        /// The error of the result if it is failed; otherwise throws an exception.
        /// </summary>
        public E Error => IsFailure
            ? _error
            : throw new InvalidOperationException("Attempted to access Error of a successful Result.");

        /// <summary>Creates a successful result with the given value.</summary>
        public static Result<T, E> Success(T value)
            => new Result<T, E>(true, value, default);

        /// <summary>Creates a failed result with the given error.</summary>
        public static Result<T, E> Failure(E error)
            => new Result<T, E>(false, default, error);

        /// <summary>
        /// Implicitly converts a value of type T to a successful Result.
        /// (Convenience for quickly returning success without explicit .Success())
        /// </summary>
        public static implicit operator Result<T, E>(T value)
            => Success(value);

        /// <summary>
        /// Implicitly converts an E (error) to a failed Result (for convenience).
        /// </summary>
        public static implicit operator Result<T, E>(E error)
            => Failure(error);
    }
}
