using System;
using System.Globalization;

namespace AggregateSource
{
    class TestFactBuilder
    {
        readonly string _identifier;
        readonly object _event;

        public TestFactBuilder()
        {
            _identifier = new Random().Next().ToString(CultureInfo.CurrentCulture);
            _event = new object();
        }

        TestFactBuilder(string identifier, object @event)
        {
            _identifier = identifier;
            _event = @event;
        }

        public TestFactBuilder WithEvent(object @event)
        {
            return new TestFactBuilder(_identifier, @event);
        }

        public TestFactBuilder WithIdentifier(string identifier)
        {
            return new TestFactBuilder(identifier, _event);
        }

        public Fact Build()
        {
            return new Fact(_identifier, _event);
        }
    }
}