using System;
using System.Threading.Tasks;

namespace AggregateSource.GEventStore
{
    /// <summary>
    /// Supports a simple iteration over an asynchronous generic collection.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate. This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.</typeparam>
    public interface IAsyncEnumerator<out T> : IDisposable
    {
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>
        /// The element in the collection at the current position of the enumerator.
        /// </value>
        T Current { get; }
        /// <summary>
        /// Advances the enumerator asynchronously to the next element of the collection.
        /// </summary>
        /// <returns><c>true</c> if the enumerator was successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.</returns>
        Task<bool> MoveNextAsync();
    }
}