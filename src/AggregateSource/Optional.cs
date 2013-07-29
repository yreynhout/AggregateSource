using System;
using System.Collections;
using System.Collections.Generic;
using AggregateSource.Properties;

namespace AggregateSource
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    public struct Optional<T> : IEnumerable<T>, IEquatable<Optional<T>>
    {
        /// <summary>
        /// The empty instance.
        /// </summary>
        public static readonly Optional<T> Empty = new Optional<T>();

        private readonly bool _hasValue;
        private readonly T _value;

        /// <summary>
        /// Initializes a new <see cref="Optional{T}"/> instance.
        /// </summary>
        /// <param name="value">The value to initialize with.</param>
        public Optional(T value)
        {
            _hasValue = true;
            _value = value;
        }

        /// <summary>
        /// Gets an indication if this instance has a value.
        /// </summary>
        public bool HasValue
        {
            get { return _hasValue; }
        }

        /// <summary>
        /// Gets the value associated with this instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this instance has no value.</exception>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException(Resources.Optional_NoValue);
                return _value;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (HasValue)
            {
                yield return _value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;
            return Equals((Optional<T>) obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Optional{T}" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Optional{T}" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="Optional{T}" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Optional<T> other)
        {
            return _hasValue.Equals(other._hasValue) &&
                   EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _hasValue.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(_value) ^ typeof (T).GetHashCode();
        }
    }
}