using System;

namespace AggregateSource.Testing
{
    /// <summary>
    /// The given-when-then test specification builder bootstrapper.
    /// </summary>
    public class Scenario : IScenarioInitialStateBuilder
    {
        /// <summary>
        /// Given the following facts occured.
        /// </summary>
        /// <param name="facts">The facts that occurred.</param>
        /// <returns>
        /// A builder continuation.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">facts</exception>
        public IScenarioGivenStateBuilder Given(params Fact[] facts)
        {
            if (facts == null) throw new ArgumentNullException("facts");
            return new TestSpecificationBuilder().Given(facts);
        }

        /// <summary>
        /// Given the following events occured.
        /// </summary>
        /// <param name="identifier">The aggregate identifier the events are to be associated with.</param>
        /// <param name="events">The events that occurred.</param>
        /// <returns>A builder continuation.</returns>
        public IScenarioGivenStateBuilder Given(string identifier, params object[] events)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (events == null) throw new ArgumentNullException("events");
            return new TestSpecificationBuilder().Given(identifier, events);
        }

        /// <summary>
        /// Given no events occured.
        /// </summary>
        /// <returns>A builder continuation.</returns>
        public IScenarioGivenNoneStateBuilder GivenNone()
        {
            return new TestSpecificationBuilder().GivenNone();
        }

        /// <summary>
        /// When a command occurs.
        /// </summary>
        /// <param name="message">The command message.</param>
        /// <returns>A builder continuation.</returns>
        public IScenarioWhenStateBuilder When(object message)
        {
            if (message == null) throw new ArgumentNullException("message");
            return new TestSpecificationBuilder().When(message);
        }
    }
}