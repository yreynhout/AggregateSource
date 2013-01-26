using System;
using System.Runtime.Remoting.Messaging;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Stores the ambient unit of work in the <see cref="CallContext"/>.
  /// </summary>
  public class CallContextUnitOfWorkStore : IAmbientUnitOfWorkStore {
    const string ItemKey = "AggregateSourceAmbientUnitOfWork";

    public void Set(UnitOfWork unitOfWork) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if (CallContext.GetData(ItemKey) != null)
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_SetAlreadySet);
      CallContext.SetData(ItemKey, unitOfWork);
    }

    public bool TryGet(out UnitOfWork unitOfWork) {
      unitOfWork = (UnitOfWork)CallContext.GetData(ItemKey);
      return unitOfWork != null;
    }

    public void Clear() {
      if (CallContext.GetData(ItemKey) == null)
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_ClearNotSet);
      CallContext.FreeNamedDataSlot(ItemKey);
    }
  }
}