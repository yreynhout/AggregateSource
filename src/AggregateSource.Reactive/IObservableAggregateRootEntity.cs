using System;

namespace AggregateSource.Reactive
{
    /// <summary>
    /// Observable aggregate root entity marker interface.
    /// </summary>
    public interface IObservableAggregateRootEntity : IAggregateRootEntity, IObservable<object>, IDisposable {}
}