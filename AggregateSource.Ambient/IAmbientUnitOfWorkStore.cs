namespace AggregateSource.Ambient {
  /// <summary>
  /// Provides access to the ambient unit of work store.
  /// </summary>
  public interface IAmbientUnitOfWorkStore {
    /// <summary>
    /// Sets the unit of work in the store.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    void Set(UnitOfWork unitOfWork);
    /// <summary>
    /// Attempts to get the currently stored unit of work, if any.
    /// </summary>
    /// <param name="unitOfWork">The currently stored <see cref="UnitOfWork"/> if any, otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if the store contains a unit of work, otherwise <c>false</c>.</returns>
    bool TryGet(out UnitOfWork unitOfWork);
    /// <summary>
    /// Clears the current unit of work from the store.
    /// </summary>
    void Clear();
  }
}
