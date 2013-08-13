using System;
using AggregateSource.Testing.AggregateBehavior.Factory;

namespace AggregateSource.Testing.AggregateBehavior
{
    /// <summary>
    /// A given-when-then test specification bootstrapper for testing an aggregate factory, i.e. a method on the aggregate that creates a new aggregate.
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of aggregate root entity under test.</typeparam>
    public class FactoryScenarioFor<TAggregateRoot> : IAggregateFactoryGivenStateBuilder<TAggregateRoot>
        where TAggregateRoot : IAggregateRootEntity
    {
        readonly Func<IAggregateRootEntity> _sutFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryScenarioFor{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="sut">The sut.</param>
        public FactoryScenarioFor(TAggregateRoot sut)
            : this(() => sut) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryScenarioFor{TAggregateRoot}"/> class.
        /// </summary>
        /// <param name="sutFactory">The sut factory.</param>
        public FactoryScenarioFor(Func<TAggregateRoot> sutFactory)
        {
            _sutFactory = () => sutFactory();
        }

        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="events"/> are <c>null</c>.</exception>
        public IAggregateFactoryGivenStateBuilder<TAggregateRoot> Given(params object[] events)
        {
            if (events == null) throw new ArgumentNullException("events");
            return new AggregateFactoryGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
        }

        /// <summary>
        /// When an aggregate is created.
        /// </summary>
        /// <param name="factory">The factory method invocation on the sut.</param>
        /// <returns>A builder continuation.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="factory"/> is <c>null</c>.</exception>
        public IAggregateFactoryWhenStateBuilder When<TAggregateRootResult>(
            Func<TAggregateRoot, TAggregateRootResult> factory) where TAggregateRootResult : IAggregateRootEntity
        {
            if (factory == null) throw new ArgumentNullException("factory");
            return new AggregateFactoryWhenStateBuilder(_sutFactory, new object[0],
                                                        root => factory((TAggregateRoot) root));
        }
    }
}