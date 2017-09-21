using System;
using System.Collections;
using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Represents an optional value.
    /// </summary>
    /// <typeparam name="T">The type of the optional value.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Optional")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public struct Optional<T> : IEnumerable<T>, IEquatable<Optional<T>>
    {
        /// <summary>
        /// The empty instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly Optional<T> Empty = new Optional<T>();

        readonly bool _hasValue;
        readonly T _value;

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
                    throw new InvalidOperationException("Optional object must have a value.");
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
            if (_hasValue.Equals(other._hasValue))
            {
                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                {
                    var enumerable1 = (IEnumerable) _value;
                    var enumerable2 = (IEnumerable) other._value;
                    if (enumerable1 == null && enumerable2 == null) return true;
                    if (enumerable1 == null || enumerable2 == null) return false;
                    var enumerator1 = enumerable1.GetEnumerator();
                    var enumerator2 = enumerable2.GetEnumerator();
                    while (enumerator1.MoveNext())
                    {
                        if (!(enumerator2.MoveNext() &&
                              EqualityComparer<object>.Default.Equals(enumerator1.Current, enumerator2.Current)))
                        {
                            return false;
                        }
                    }
                    return !enumerator2.MoveNext();
                }
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            }
            return false;
        }

        /// <summary>
        /// Determines whether <see cref="Optional{T}">instance 1</see> is equal to <see cref="Optional{T}">instance 2</see>.
        /// </summary>
        /// <param name="instance1">The first instance.</param>
        /// <param name="instance2">The second instance.</param>
        /// <returns><c>true</c> if <see cref="Optional{T}">instance 1</see> is equal to <see cref="Optional{T}">instance 2</see>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Optional<T> instance1, Optional<T> instance2)
        {
            return instance1.Equals(instance2);
        }

        /// <summary>
        /// Determines whether <see cref="Optional{T}">instance 1</see> is not equal to <see cref="Optional{T}">instance 2</see>.
        /// </summary>
        /// <param name="instance1">The first instance.</param>
        /// <param name="instance2">The second instance.</param>
        /// <returns><c>true</c> if <see cref="Optional{T}">instance 1</see> is not equal to <see cref="Optional{T}">instance 2</see>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Optional<T> instance1, Optional<T> instance2)
        {
            return !instance1.Equals(instance2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                var enumerable = (IEnumerable)_value;
                if (enumerable != null)
                {
                    var enumerator = enumerable.GetEnumerator();
                    var hashCode = _hasValue.GetHashCode();
                    while (enumerator.MoveNext())
                    {
                        hashCode ^= EqualityComparer<object>.Default.GetHashCode(enumerator.Current);
                    }
                    return hashCode ^ typeof(T).GetHashCode();
                }
            }
            return _hasValue.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(_value) ^ typeof (T).GetHashCode();
        }
    }
}