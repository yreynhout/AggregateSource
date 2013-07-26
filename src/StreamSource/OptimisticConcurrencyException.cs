using System;
using System.Runtime.Serialization;

namespace StreamSource
{
    [Serializable]
    public class OptimisticConcurrencyException : StreamSourceException
    {
        readonly Guid _streamId;
        readonly int _expectedVersion;

        public OptimisticConcurrencyException(Guid streamId, Int32 expectedVersion)
        {
            _streamId = streamId;
            _expectedVersion = expectedVersion;
        }

        public OptimisticConcurrencyException(Guid streamId, Int32 expectedVersion, string message)
            : base(message)
        {
            _streamId = streamId;
            _expectedVersion = expectedVersion;
        }

        public OptimisticConcurrencyException(Guid streamId, Int32 expectedVersion, string message,
                                              Exception innerException)
            : base(message, innerException)
        {
            _streamId = streamId;
            _expectedVersion = expectedVersion;
        }

        protected OptimisticConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _streamId = new Guid(info.GetString("StreamId"));
            _expectedVersion = info.GetInt32("ExpectedVersion");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("StreamId", _streamId.ToString("N"));
            info.AddValue("ExpectedVersion", _expectedVersion);
        }

        public Guid StreamId
        {
            get { return _streamId; }
        }

        public Int32 ExpectedVersion
        {
            get { return _expectedVersion; }
        }
    }
}