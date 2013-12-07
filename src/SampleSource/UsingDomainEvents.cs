using System;
using System.Collections.Generic;
using AggregateSource;
using NUnit.Framework;

namespace SampleSource
{
    namespace UsingDomainEvents
    {
        using Messaging;

        [TestFixture]
        public class SampleUsage
        {
            [Test]
            public void Test()
            {
                var dispatched = false;
                using (DomainEventDispatcher.Subscribe<AddedNewTodoList>(@event => { dispatched = true; }))
                {
                    var _ = new TodoList(
                        new TodoListId(Guid.NewGuid()), "Before my 40th birthday");
                }
                Assert.IsTrue(dispatched);
            }
        }

        public class DomainEventAwareAggregateRootEntity : AggregateRootEntity
        {
            protected override void BeforeApplyChange(object @event)
            {
                DomainEventDispatcher.Publish(@event);
            }
        }

        public static class DomainEventDispatcher
        {
            [ThreadStatic] static Dictionary<Type, List<Action<object>>> _index;

            public static IDisposable Subscribe<TEvent>(Action<TEvent> handler)
            {
                if (handler == null) throw new ArgumentNullException("handler");
                if (_index == null)
                    _index = new Dictionary<Type, List<Action<object>>>();
                List<Action<object>> handlers;
                if (!_index.TryGetValue(typeof (TEvent), out handlers))
                {
                    handlers = new List<Action<object>>();
                    _index.Add(typeof (TEvent), handlers);
                }
                var action = new Action<Object>(@event => handler((TEvent) @event));
                handlers.Add(action);
                return new Disposable(() => handlers.Remove(action));
            }

            public static void Publish(object @event)
            {
                if (@event == null) throw new ArgumentNullException("event");
                List<Action<object>> handlers;
                if (!_index.TryGetValue(@event.GetType(), out handlers)) return;
                foreach (var handler in handlers)
                {
                    handler(@event);
                }
            }

            class Disposable : IDisposable
            {
                readonly Action _disposer;
                bool _disposed;

                public Disposable(Action disposer)
                {
                    if (disposer == null) throw new ArgumentNullException("disposer");
                    _disposer = disposer;
                }

                public void Dispose()
                {
                    if (_disposed) return;
                    _disposer();
                    _disposed = true;
                }
            }
        }

        public class TodoList : DomainEventAwareAggregateRootEntity
        {
            TodoList() {}

            public TodoList(TodoListId id, string name)
                : this()
            {
                ApplyChange(new AddedNewTodoList(id, name));
            }

            // state
        }

        public struct TodoListId : IEquatable<TodoListId>
        {
            readonly Guid _value;

            public TodoListId(Guid value)
            {
                _value = value;
            }

            public bool Equals(TodoListId other)
            {
                return _value.Equals(other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is TodoListId && Equals((TodoListId) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator Guid(TodoListId id)
            {
                return id._value;
            }
        }

        namespace Messaging
        {
            public class AddedNewTodoList
            {
                protected bool Equals(AddedNewTodoList other)
                {
                    return Id.Equals(other.Id) && string.Equals(Name, other.Name);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((AddedNewTodoList) obj);
                }

                public override int GetHashCode()
                {
                    return Id.GetHashCode() ^ (Name == null ? 0 : Name.GetHashCode());
                }

                public readonly Guid Id;
                public readonly string Name;

                public AddedNewTodoList(Guid id, string name)
                {
                    Id = id;
                    Name = name;
                }
            }
        }
    }
}