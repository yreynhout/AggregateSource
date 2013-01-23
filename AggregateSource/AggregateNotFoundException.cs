using System;
using System.Runtime.Serialization;
using AggregateSource.Properties;

namespace AggregateSource {
  [Serializable]
  public class AggregateNotFoundException : AggregateSourceException {
    readonly Guid _aggregateId;
    readonly Type _aggregateType;

    public AggregateNotFoundException(Guid aggregateId, Type aggregateType) 
      : base(string.Format(Resources.AggregateNotFoundException_DefaultMessage, aggregateType != null ? aggregateType.Name : "", aggregateId) ) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    public AggregateNotFoundException(Guid aggregateId, Type aggregateType, string message) 
      : base(message) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    public AggregateNotFoundException(Guid aggregateId, Type aggregateType, string message, Exception innerException)
      : base(message, innerException) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    protected AggregateNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context) {
      _aggregateId = new Guid(info.GetString("AggregateId"));
      _aggregateType = Type.GetType(info.GetString("AggregateType"), false);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      info.AddValue("AggregateId", _aggregateId.ToString());
      info.AddValue("AggregateType", _aggregateType.AssemblyQualifiedName);
    }

    public Guid AggregateId {
      get { return _aggregateId; }
    }

    public Type AggregateType {
      get { return _aggregateType; }
    }
  }
}