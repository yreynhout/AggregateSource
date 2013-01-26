using System;
using System.Threading;
using System.Web;
using AggregateSource.Ambient.Properties;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Stores the ambient unit of work in the <see cref="HttpContext"/>.
  /// </summary>
  public class HttpContextUnitOfWorkStore : IAmbientUnitOfWorkStore {
    const string ItemKey = "AggregateSourceAmbientUnitOfWork";

    public void Set(UnitOfWork unitOfWork) {
      if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
      if(HttpContext.Current.Items.Contains(ItemKey))
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_SetAlreadySet);
      HttpContext.Current.Items.Add(ItemKey, unitOfWork);
    }

    public bool TryGet(out UnitOfWork unitOfWork) {
      unitOfWork = (UnitOfWork) HttpContext.Current.Items[ItemKey];
      return unitOfWork != null;
    }

    public void Clear() {
      if (!HttpContext.Current.Items.Contains(ItemKey))
        throw new UnitOfWorkScopeException(Resources.AmbientUnitOfWorkStore_SetAlreadySet);
      HttpContext.Current.Items.Remove(ItemKey);
    }
  }
}