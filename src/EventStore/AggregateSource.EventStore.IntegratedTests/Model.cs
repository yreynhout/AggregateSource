using System;

namespace AggregateSource.EventStore
{
    public class Model
    {
        public Model()
        {
            KnownIdentifier = "aggregate/" + Guid.NewGuid();
            UnknownIdentifier = "aggregate/" + Guid.NewGuid();
        }

        public string KnownIdentifier { get; private set; }
        public string UnknownIdentifier { get; private set; }
    }
}