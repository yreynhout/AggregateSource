using System;
using AggregateSource.Properties;

namespace AggregateSource {
  public class Aggregate {
    public static readonly int InitialVersion = 0;

    readonly Guid _id;
    readonly int _version;
    readonly AggregateRootEntity _root;

    public Aggregate(Guid id, int version, AggregateRootEntity root) {
      if (root == null) 
        throw new ArgumentNullException("root");
      if (version < InitialVersion) 
        throw new ArgumentOutOfRangeException("version",
                                              string.Format(Resources.Aggregate_VersionOutOfRange, InitialVersion));
      _id = id;
      _version = version;
      _root = root;
    }

    public Guid Id {
      get { return _id; }
    }

    public int Version {
      get { return _version; }
    }

    public AggregateRootEntity Root {
      get { return _root; }
    }
  }
}