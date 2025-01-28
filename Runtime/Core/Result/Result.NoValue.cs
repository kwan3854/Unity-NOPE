using System;

namespace NOPE.Runtime.Core.Result
{
    /// <summary>
    /// Represents a success/failure result with no attached value, only an error of type E if failed.
    /// Similar to "void" version of <see cref="Result{T,E}"/>.
    /// </summary>
    public readonly partial struct Result<E>
    {
        private readonly E _error;

        private Result(bool isSuccess, E error)
        {
            IsSuccess = isSuccess;
            _error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public E Error => IsFailure
            ? _error
            : throw new InvalidOperationException("Attempted to access Error of a successful Result.");

        public static Result<E> Success()
            => new Result<E>(true, default);

        public static Result<E> Failure(E error)
            => new Result<E>(false, error);

        public static implicit operator Result<E>(E error)
            => Failure(error);
    }
}