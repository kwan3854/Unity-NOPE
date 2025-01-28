using NOPE.Runtime.Core.Result;

namespace NOPE.Runtime.Core
{
    /// <summary>
    /// A simple "Unit" type representing "no meaningful value".
    /// Used for <see cref="Result{T,E}"/> when no success value is needed.
    /// </summary>
    public struct Unit 
    {
        public static readonly Unit Value = default;
    }
}