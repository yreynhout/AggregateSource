using System;

namespace AggregateSource.Testing {
  /// <summary>
  /// A given-when-then test specification bootstrapper for testing an aggregate command, i.e. a method on the aggregate that returns void.
  /// </summary>
  /// <typeparam name="TAggregateRoot">The type of aggregate root entity under test.</typeparam>
  public class CommandScenarioFor<TAggregateRoot> : IAggregateCommandGivenStateBuilder<TAggregateRoot> where TAggregateRoot : IAggregateRootEntity {
    readonly Func<IAggregateRootEntity> _sutFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandScenarioFor{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="sut">The sut.</param>
    public CommandScenarioFor(TAggregateRoot sut)
      : this(() => sut) {}

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandScenarioFor{TAggregateRoot}"/> class.
    /// </summary>
    /// <param name="sutFactory">The sut factory.</param>
    public CommandScenarioFor(Func<TAggregateRoot> sutFactory) {
      _sutFactory = () => sutFactory();
    }

    /// <summary>
    /// Given the following events occured.
    /// </summary>
    /// <param name="events">The events that occurred.</param>
    /// <returns>A builder continuation.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="events"/> are <c>null</c>.</exception>
    public IAggregateCommandGivenStateBuilder<TAggregateRoot> Given(params object[] events) {
      if (events == null) throw new ArgumentNullException("events");
      return new AggregateCommandGivenStateBuilder<TAggregateRoot>(_sutFactory, events);
    }

    /// <summary>
    /// When a command occurs.
    /// </summary>
    /// <param name="command">The command method invocation on the sut.</param>
    /// <returns>A builder continuation.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="command"/> is <c>null</c>.</exception>
    public IAggregateCommandWhenStateBuilder When(Action<TAggregateRoot> command) {
      if (command == null) throw new ArgumentNullException("command");
      return new AggregateCommandWhenStateBuilder(_sutFactory, new object[0], root => command((TAggregateRoot)root));
    }
  }
}