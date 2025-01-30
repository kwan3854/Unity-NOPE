
namespace NOPE.Runtime.Core.Result
{
    public static partial class ResultExtensions
    {
        public static bool TryGetValue<T, E>(this Result<T, E> result, out T value)
        {
            value = default;
            if (result.IsFailure) return false;
            value = result.Value;
            return true;
        }
        
        public static bool TryGetError<T, E>(this Result<T, E> result, out E error)
        {
            error = default;
            if (result.IsSuccess) return false;
            error = result.Error;
            return true;
        }
    }
}
