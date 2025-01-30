
namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeExtensions
    {
        /// <summary>
        /// Tries to get the value from a <see cref="Maybe{T}"/>.
        /// </summary>
        /// <returns>True if the Maybe has a value, false otherwise.</returns>
        public static bool TryGetValue<T>(this Maybe<T> maybe, out T value)
        {
            value = default;
            if (!maybe.HasValue) return false;
            value = maybe.Value;
            return true;
        }
    }
}
