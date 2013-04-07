using System;
using System.Runtime.Serialization;

namespace StreamSource {
  [Serializable]
  public class OptimisticCausationException : StreamSourceException {
    private readonly Guid _causationId;

    public OptimisticCausationException(Guid causationId) {
      _causationId = causationId;
    }

    public OptimisticCausationException(Guid causationId, string message)
      : base(message) {
      _causationId = causationId;
    }

    public OptimisticCausationException(Guid causationId, string message, Exception innerException)
      : base(message, innerException) {
      _causationId = causationId;
    }

    protected OptimisticCausationException(SerializationInfo info, StreamingContext context) : base(info, context) {
      _causationId = new Guid(info.GetString("CausationId"));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      info.AddValue("CausationId", _causationId.ToString("N"));
    }

    public Guid CausationId {
      get { return _causationId; }
    }
  }
}
