using System;

namespace AggregateSource
{
    /// <summary>
    /// Base class for aggregate entities that need some basic infrastructure for tracking state changes on their aggregate root entity.
    /// </summary>
    public abstract class Entity<TEntityState> : IInstanceEventRouter
        where TEntityState : new()
    {
        readonly Action<object> _applier;
        readonly TEntityState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity{TEntityState}"/> class.
        /// </summary>
        /// <param name="applier">The event player and recorder.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="applier"/> is null.</exception>
        protected Entity(Action<object> applier)
        {
            if (applier == null) throw new ArgumentNullException("applier");
            _applier = applier;
            _state = new TEntityState();
        }

        /// <summary>
        /// Gets the entity state associated with this instance.
        /// </summary>
        /// <value>
        /// The entity state.
        /// </value>
        protected TEntityState State 
        {
          get { return _state; }
        }

        /// <summary>
        /// Routes the specified <paramref name="event"/> to a configured state handler, if any.
        /// </summary>
        /// <param name="event">The event to route.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="event"/> is null.</exception>
        public void Route(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            ((dynamic)_state).When((dynamic)@event);
        }

        /// <summary>
        /// Applies the specified event to this instance and invokes the associated state handler.
        /// </summary>
        /// <param name="event">The event to apply.</param>
        protected void Apply(object @event)
        {
            if (@event == null) throw new ArgumentNullException("event");
            _applier(@event);
        }
    }
}