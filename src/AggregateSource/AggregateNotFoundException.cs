using System;
using System.Runtime.Serialization;
using AggregateSource.Properties;

namespace AggregateSource
{
    /// <summary>
    /// Exception that tells callers an aggregate was not found.
    /// </summary>
    [Serializable]
    public class AggregateNotFoundException : AggregateSourceException
    {
        private readonly string _identifier;
        private readonly Type _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="type">Type of the aggregate root entity.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="type"/> is null.</exception>
        public AggregateNotFoundException(string identifier, Type type)
            : base(
                type != null && identifier != null
                    ? string.Format(Resources.AggregateNotFoundException_DefaultMessage, type.Name, identifier)
                    : null)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (type == null) throw new ArgumentNullException("type");
            _identifier = identifier;
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="type">Type of the aggregate root entity.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="type"/> is null.</exception>
        public AggregateNotFoundException(string identifier, Type type, string message)
            : base(message)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (type == null) throw new ArgumentNullException("type");
            _identifier = identifier;
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The aggregate identifier.</param>
        /// <param name="type">Type of the aggregate root entity.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <paramref name="type"/> is null.</exception>
        public AggregateNotFoundException(string identifier, Type type, string message, Exception innerException)
            : base(message, innerException)
        {
            if (identifier == null) throw new ArgumentNullException("identifier");
            if (type == null) throw new ArgumentNullException("type");
            _identifier = identifier;
            _type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected AggregateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _identifier = info.GetString("identifier");
            _type = Type.GetType(info.GetString("type"), false);
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
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("identifier", _identifier);
            info.AddValue("type", _type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Gets the aggregate id.
        /// </summary>
        /// <value>
        /// The aggregate id.
        /// </value>
        public string Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Gets the type of the aggregate root entity.
        /// </summary>
        /// <value>
        /// The type of the aggregate root entity.
        /// </value>
        public Type Type
        {
            get { return _type; }
        }
    }
}