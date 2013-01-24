using System;

namespace AggregateSource {
  public class Aggregate {
    readonly Guid _id;
    readonly AggregateRootEntity _root;

    public Aggregate(Guid id, AggregateRootEntity root) {
      if (root == null) 
        throw new ArgumentNullException("root");
      _id = id;
      _root = root;
    }

    public Guid Id {
      get { return _id; }
    }

    public AggregateRootEntity Root {
      get { return _root; }
    }
  }
}