using System;
using System.Runtime.Serialization;

namespace AggregateSource.Ambient {
  /// <summary>
  /// Exception thrown when there's a problem related to a <see cref="UnitOfWorkScope"/>.
  /// </summary>
  [Serializable]
  public class UnitOfWorkScopeException : AggregateSourceException {
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScopeException"/> class.
    /// </summary>
    public UnitOfWorkScopeException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScopeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnitOfWorkScopeException(string message)
      : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScopeException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public UnitOfWorkScopeException(string message, Exception innerException)
      : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkScopeException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    protected UnitOfWorkScopeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}