using System;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Stores the ambient unit of work in a thread static field.
  /// </summary>
  public class ThreadStaticUnitOfWorkStore : IAmbientUnitOfWorkStore {
    [ThreadStatic]
    static UnitOfWork _store;

    public void Set(UnitOfWork unitOfWork) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if(_store != null)
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_SetAlreadySet);
      _store = unitOfWork;
    }

    public bool TryGet(out UnitOfWork unitOfWork) {
      unitOfWork = _store;
      return unitOfWork != null;
    }

    public void Clear() {
      if(_store == null)
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_ClearNotSet);
      _store = null;
    }
  }
}