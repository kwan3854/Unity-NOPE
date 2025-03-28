using System;
using System.Collections.Generic;

namespace NOPE.Runtime.Core.Maybe
{
    /// <summary>
    /// Represents an optional value: either it has a value (of type T) or it does not (None).
    /// </summary>
    public readonly struct Maybe<T> : IEquatable<Maybe<T>>
    {
        private readonly bool _hasValue;
        private readonly T _value;

        private Maybe(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        /// <summary>
        /// Whether this maybe actually contains a value.
        /// </summary>
        public bool HasValue => _hasValue;

        /// <summary>
        /// Whether this maybe does not contain a value.
        /// </summary>
        public bool HasNoValue => !_hasValue;

        /// <summary>
        /// Returns the contained value if it exists; otherwise throws an exception.
        /// </summary>
        public T Value => _hasValue
            ? _value
            : throw new InvalidOperationException("No value in Maybe.");

        /// <summary>
        /// Constructs a Maybe from a given value (if non-null).
        /// If the value is null, returns None.
        /// </summary>
        public static Maybe<T> From(T value)
        {
            if (value == null)
                return None;
            return new Maybe<T>(value, true);
        }

        /// <summary>
        /// A Maybe that has no value.
        /// </summary>
        public static Maybe<T> None => new Maybe<T>(default, false);

        /// <summary>
        /// Implicitly converts a value of type T to Maybe&lt;T&gt; (useful for direct returns).
        /// </summary>
        public static implicit operator Maybe<T>(T value) => From(value);
        
        public override bool Equals(object obj)
        {
            return obj is Maybe<T> other && Equals(other);
        }
        
        public bool Equals(Maybe<T> other)
        {
            if (!this._hasValue && !other._hasValue)
                return true;
            
            if (this._hasValue != other._hasValue)
                return false;
            
            return EqualityComparer<T>.Default.Equals(this._value, other._value);
        }
        
        public override int GetHashCode()
        {
            return _hasValue ? _value.GetHashCode() : 0;
        }
        
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }
    }
}