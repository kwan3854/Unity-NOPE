
namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeExtensions
    {
        public static bool TryGetValue<T>(this Maybe<T> maybe, out T value)
        {
            value = default;
            if (!maybe.HasValue) return false;
            value = maybe.Value;
            return true;
        }
    }
}
