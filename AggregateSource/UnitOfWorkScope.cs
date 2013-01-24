using System;
using AggregateSource.Properties;

namespace AggregateSource {
  /// <summary>
  /// Makes a block of code aware of an ambient <see cref="UnitOfWork"/>.
  /// </summary>
  public class UnitOfWorkScope : IDisposable {
    readonly UnitOfWork _unitOfWork;
    bool _disposed;

    [ThreadStatic]
    static UnitOfWorkScope _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScope"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <exception cref="System.ArgumentNullException">unitOfWork</exception>
    public UnitOfWorkScope(UnitOfWork unitOfWork) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      _unitOfWork = unitOfWork;
      PushScope(this);
    }

    /// <summary>
    /// Gets the unit of work that has been scoped.
    /// </summary>
    /// <value>
    /// The scoped unit of work.
    /// </value>
    public UnitOfWork UnitOfWork {
      get { return _unitOfWork; }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
      if (!_disposed) {
        _disposed = true;
        PopScope();
      }
    }

    static void PushScope(UnitOfWorkScope scope) {
      if (IsScoped) throw new UnitOfWorkScopeException(Resources.UnitOfWorkScope_PushScopeConflict);
      _currentScope = scope;
    }

    static void PopScope() {
      _currentScope = null;
    }

    /// <summary>
    /// Gets a value indicating whether a <see cref="UnitOfWorkScope"/> is scoped.
    /// </summary>
    /// <value>
    ///   <c>true</c> if a <see cref="UnitOfWorkScope"/> is scoped; otherwise, <c>false</c>.
    /// </value>
    public static bool IsScoped {
      get { return !ReferenceEquals(_currentScope, null); }
    }

    /// <summary>
    /// Gets the currently scoped <see cref="UnitOfWorkScope"/>.
    /// </summary>
    /// <value>
    /// The currently scoped <see cref="UnitOfWorkScope"/>.
    /// </value>
    /// <exception cref="UnitOfWorkScopeException">Thrown when there is no currently scoped <see cref="UnitOfWorkScope"/>.</exception>
    public static UnitOfWorkScope Current {
      get {
        if (!IsScoped)
          throw new UnitOfWorkScopeException(Resources.UnitOfWorkScope_CurrentNotScoped);
        return _currentScope;
      }
    }

    /// <summary>
    /// Attempts to get the currently scoped <see cref="UnitOfWorkScope"/>.
    /// </summary>
    /// <param name="scope">The currently scoped <see cref="UnitOfWorkScope"/>, otherwise <c>null</c>.</param>
    /// <returns><c>true</c> if there is a scoped <see cref="UnitOfWorkScope"/>, otherwise <c>false</c>.</returns>
    public static bool TryGetCurrent(out UnitOfWorkScope scope) {
      if (IsScoped) {
        scope = _currentScope;
        return true;
      }
      scope = null;
      return false;
    }
  }
}
