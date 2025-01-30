
namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Tries to get the value from a <see cref="Result{T, E}"/>.
        /// </summary>
        /// <returns>True if the Result is successful, false otherwise.</returns>
        public static bool TryGetValue<T, E>(this Result<T, E> result, out T value)
        {
            value = default;
            if (result.IsFailure) return false;
            value = result.Value;
            return true;
        }
        
        /// <summary>
        /// Tries to get the error from a <see cref="Result{T, E}"/>.
        /// </summary>
        /// <returns>True if the Result is failed, false otherwise.</returns>
        public static bool TryGetError<T, E>(this Result<T, E> result, out E error)
        {
            error = default;
            if (result.IsSuccess) return false;
            error = result.Error;
            return true;
        }
    }
}
