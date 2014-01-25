using System.Collections.Generic;

namespace AggregateSource
{
    /// <summary>
    /// Tracks changes that happen to an aggregate
    /// </summary>
    public interface IAggregateChangeTracker
    {
        /// <summary>
        /// Determines whether this instance has state changes.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has state changes; otherwise, <c>false</c>.
        /// </returns>
        bool HasChanges();

        /// <summary>
        /// Gets the state changes applied to this instance.
        /// </summary>
        /// <returns>A list of recorded state changes.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        IEnumerable<object> GetChanges();

        /// <summary>
        /// Clears the state changes.
        /// </summary>
        void ClearChanges();
    }
}