using System;
using System.Collections.Generic;
using UnityEngine;

#if NOPE_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace NOPE.Runtime.Core.Maybe
{
    public static partial class MaybeDictionaryExtensions
    {
        /// <summary>
        /// Tries to find a value in the dictionary by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dict">The dictionary to search.</param>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the value if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the dictionary is null.</exception>
        public static Maybe<TValue> TryFind<TKey, TValue>(
            this IDictionary<TKey, TValue> dict,
            TKey key)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            return dict.TryGetValue(key, out var value) 
                ? Maybe<TValue>.From(value) 
                : Maybe<TValue>.None;
        }

#if NOPE_UNITASK
        /// <summary>
        /// Tries to find a value in the dictionary by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="asyncDict">The dictionary to search.</param>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the value if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the dictionary is null.</exception>
        public static async UniTask<Maybe<TValue>> TryFind<TKey, TValue>(
            this UniTask<IDictionary<TKey, TValue>> asyncDict,
            TKey key)
        {
            var dict = await asyncDict;
            if (dict == null) 
                throw new ArgumentNullException(nameof(dict));
            return dict.TryGetValue(key, out var value) 
                ? Maybe<TValue>.From(value) 
                : Maybe<TValue>.None;
        }
#endif

#if NOPE_AWAITABLE
        /// <summary>
        /// Tries to find a value in the dictionary by the specified key.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="asyncDict">The dictionary to search.</param>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>A <see cref="Maybe{T}"/> containing the value if found; otherwise, <see cref="Maybe{T}.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the dictionary is null.</exception>
        public static async Awaitable<Maybe<TValue>> TryFind<TKey, TValue>(
            this Awaitable<IDictionary<TKey, TValue>> asyncDict,
            TKey key)
        {
            var dict = await asyncDict;
            if (dict == null) 
                throw new ArgumentNullException(nameof(dict));
            return dict.TryGetValue(key, out var value) 
                ? Maybe<TValue>.From(value) 
                : Maybe<TValue>.None;
        }
#endif
    }
}
