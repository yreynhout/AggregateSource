using System;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
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

    static bool IsScoped {
      get { return !ReferenceEquals(_currentScope, null); }
    }

    internal static bool TryGetCurrent(out UnitOfWorkScope scope) {
      if (IsScoped) {
        scope = _currentScope;
        return true;
      }
      scope = null;
      return false;
    }
  }
}
