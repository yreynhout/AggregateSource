using System;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Makes a block of code aware of an ambient <see cref="UnitOfWork"/>.
  /// </summary>
  public class UnitOfWorkScope : IDisposable {
    readonly UnitOfWork _unitOfWork;
    readonly IAmbientUnitOfWorkStore _store;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScope"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="store">The local storage of the unit of work.</param>
    /// <exception cref="System.ArgumentNullException">unitOfWork</exception>
    public UnitOfWorkScope(UnitOfWork unitOfWork, IAmbientUnitOfWorkStore store) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (store == null) throw new ArgumentNullException("store");
      _unitOfWork = unitOfWork;
      _store = store;
      _store.Set(_unitOfWork);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      if (!_disposed) {
        _store.Clear();
        _disposed = true;
      }
    }
  }
}
