using System;
using System.Runtime.Serialization;
using AggregateSource.Properties;

namespace AggregateSource {
  /// <summary>
  /// Exception that tells callers an aggregate was not found.
  /// </summary>
  [Serializable]
  public class AggregateNotFoundException : AggregateSourceException {
    readonly Guid _aggregateId;
    readonly Type _aggregateType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateType">Type of the aggregate root entity.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregateType"/> is null.</exception>
    public AggregateNotFoundException(Guid aggregateId, Type aggregateType) 
      : base(string.Format(Resources.AggregateNotFoundException_DefaultMessage, aggregateType != null ? aggregateType.Name : "", aggregateId) ) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateType">Type of the aggregate root entity.</param>
    /// <param name="message">The message.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregateType"/> is null.</exception>
    public AggregateNotFoundException(Guid aggregateId, Type aggregateType, string message) 
      : base(message) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
    /// </summary>
    /// <param name="aggregateId">The aggregate id.</param>
    /// <param name="aggregateType">Type of the aggregate root entity.</param>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="aggregateType"/> is null.</exception>
    public AggregateNotFoundException(Guid aggregateId, Type aggregateType, string message, Exception innerException)
      : base(message, innerException) {
      if (aggregateType == null) throw new ArgumentNullException("aggregateType");
      _aggregateId = aggregateId;
      _aggregateType = aggregateType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The info.</param>
    /// <param name="context">The context.</param>
    protected AggregateNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context) {
      _aggregateId = new Guid(info.GetString("AggregateId"));
      _aggregateType = Type.GetType(info.GetString("AggregateType"), false);
    }

    /// <summary>
    /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
    /// <PermissionSet>
    ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
    ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
    ///   </PermissionSet>
    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
      base.GetObjectData(info, context);
      info.AddValue("AggregateId", _aggregateId.ToString());
      info.AddValue("AggregateType", _aggregateType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Gets the aggregate id.
    /// </summary>
    /// <value>
    /// The aggregate id.
    /// </value>
    public Guid AggregateId {
      get { return _aggregateId; }
    }

    /// <summary>
    /// Gets the type of the aggregate root entity.
    /// </summary>
    /// <value>
    /// The type of the aggregate root entity.
    /// </value>
    public Type AggregateType {
      get { return _aggregateType; }
    }
  }
}