using System;
using AggregateSource.Properties;

namespace AggregateSource {
  /// <summary>
  /// Base class for tracking aggregate meta data and its <see cref="AggregateRootEntity"/>.
  /// </summary>
  public class Aggregate {
    /// <summary>
    /// The initial version of an aggregate.
    /// </summary>
    public static readonly int InitialVersion = 0;

    readonly Guid _id;
    readonly AggregateRootEntity _root;
    readonly int _version;

    /// <summary>
    /// Initializes a new instance of the <see cref="Aggregate"/> class.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="version">The aggregate version.</param>
    /// <param name="root">The aggregate root entity.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="root"/> is null.</exception>
    public Aggregate(Guid id, int version, AggregateRootEntity root) {
      if (root == null) 
        throw new ArgumentNullException("root");
      if(version < InitialVersion)
        throw new ArgumentOutOfRangeException("version", 
          string.Format(Resources.Aggregate_VersionGreaterThanOrEqualToInitialVersion, InitialVersion));
      _id = id;
      _version = version;
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
    public Int32 Version {
      get { return _version; }
    }

    /// <summary>
    /// Gets the aggregate root entity.
    /// </summary>
    /// <value>
    /// The aggregate root entity.
    /// </value>
    public AggregateRootEntity Root {
      get { return _root; }
    }   
  }
}