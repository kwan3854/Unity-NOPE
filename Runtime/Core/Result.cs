using System;

namespace NOPE.Runtime
{
    /// <summary>
    /// Represents the outcome of an operation that can either succeed (with a value)
    /// or fail (with an error message).
    /// </summary>
    public readonly struct Result<T>
    {
        private readonly T _value;
        private readonly string _error;

        private Result(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            _value = value;
            _error = error;
        }

        /// <summary>
        /// Whether this result represents a successful outcome.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Whether this result represents a failed outcome.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The value of the result if it is successful; otherwise throws an exception.
        /// </summary>
        public T Value => IsSuccess
            ? _value
            : throw new InvalidOperationException("Attempted to access Value of a failed Result.");

        /// <summary>
        /// The error message of the result if it is failed; otherwise throws an exception.
        /// </summary>
        public string Error => IsFailure
            ? _error
            : throw new InvalidOperationException("Attempted to access Error of a successful Result.");

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        public static Result<T> Success(T value)
            => new Result<T>(true, value, default);

        /// <summary>
        /// Creates a failed result with the given error message.
        /// </summary>
        public static Result<T> Failure(string error)
            => new Result<T>(false, default, error);

        /// <summary>
        /// Implicitly converts a value of type T to a successful Result.
        /// </summary>
        public static implicit operator Result<T>(T value) => Success(value);

        /// <summary>
        /// Implicitly converts a string to a failed Result (for convenience).
        /// </summary>
        public static implicit operator Result<T>(string error) => Failure(error);
    }
}