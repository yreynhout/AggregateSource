using System;
using System.Runtime.Serialization;

namespace AggregateSource {
  [Serializable]
  public class AggregateSourceException : Exception {
    public AggregateSourceException() {}

    public AggregateSourceException(string message) 
      : base(message) {}

    public AggregateSourceException(string message, Exception innerException) 
      : base(message, innerException) {}

    protected  AggregateSourceException(SerializationInfo info, StreamingContext context) : base(info, context) {}
  }
}