using System;
using System.Collections;
using System.Collections.Generic;
using AggregateSource.Properties;

namespace AggregateSource {
  /// <summary>
  /// Represents an optional value.
  /// </summary>
  /// <typeparam name="T">The type of the optional value.</typeparam>
  public struct Optional<T> : IEnumerable<T> {
    /// <summary>
    /// The empty instance.
    /// </summary>
    public static readonly Optional<T> Empty = new Optional<T>();

    readonly bool _hasValue;
    readonly T _value;
   
    /// <summary>
    /// Initializes a new <see cref="Optional{T}"/> instance.
    /// </summary>
    /// <param name="value">The value to initialize with.</param>
    public Optional(T value) {
      _hasValue = true;
      _value = value;
    }

    /// <summary>
    /// Gets an indication if this instance has a value.
    /// </summary>
    public bool HasValue {
      get { return _hasValue; }
    }

    /// <summary>
    /// Gets the value associated with this instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when this instance has no value.</exception>
    public T Value {
      get {
        if(!HasValue)
          throw new InvalidOperationException(Resources.Optional_NoValue);
        return _value;
      }
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() {
      if (HasValue) {
        yield return _value;
      }
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode() {
      return _hasValue.GetHashCode() ^ EqualityComparer<T>.Default.GetHashCode(_value);
    }
  }
}
