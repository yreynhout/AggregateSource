using System;

namespace AggregateSource {
  /// <summary>
  /// Base class for tracking aggregate meta data and its <see cref="AggregateRootEntity"/>.
  /// </summary>
  public class Aggregate {
    readonly Guid _id;
    readonly IAggregateRootEntity _root;
    readonly int _expectedVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregate"/> class.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="expectedVersion">The expected aggregate version.</param>
    /// <param name="root">The aggregate root entity.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="root"/> is null.</exception>
    public Aggregate(Guid id, int expectedVersion, IAggregateRootEntity root) {
      if (root == null) 
        throw new ArgumentNullException("root");
      _id = id;
      _expectedVersion = expectedVersion;
      _root = root;
    }

    /// <summary>
    /// Gets the aggregate identifier.
    /// </summary>
    /// <value>
    /// The aggregate identifier.
    /// </value>
    public Guid Id {
      get { return _id; }
    }

    /// <summary>
    /// Gets the aggregate version.
    /// </summary>
    public Int32 ExpectedVersion {
      get { return _expectedVersion; }
    }

    /// <summary>
    /// Gets the aggregate root entity.
    /// </summary>
    /// <value>
    /// The aggregate root entity.
    /// </value>
    public IAggregateRootEntity Root {
      get { return _root; }
    }   
  }
}