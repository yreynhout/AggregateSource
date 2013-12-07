using System;
using System.Collections.Generic;
using System.Linq;
using AggregateSource;
using AggregateSource.Testing;
using NUnit.Framework;

namespace SampleSource.Testing
{
    namespace UsingIdFromIEvent
    {
        using Messaging;

        [TestFixture]
        public class TodoListTesting
        {
            public static readonly Guid TodoListId = Guid.NewGuid();

            [Test]
            public void when_adding_a_new_list()
            {
                new Scenario().
                    When(new AddNewTodoList(TodoListId, "Things I should do when I'm a hundred years old.")).
                    Then(new AddedNewTodoList(TodoListId, "Things I should do when I'm a hundred years old.")).
                    Assert();
            }
        }

        public static class ScenarioExtensions
        {
            public static IScenarioGivenStateBuilder Given(this IScenarioGivenStateBuilder builder, params IEvent[] events)
            {
                if (events == null) throw new ArgumentNullException("events");
                return events.Aggregate(builder, (current, @event) => current.Given(@event.Id.ToString(), @event));
            }

            public static IScenarioThenStateBuilder Then(this IScenarioWhenStateBuilder builder, params IEvent[] events)
            {
                if (events == null) throw new ArgumentNullException("events");
                if (events.Length == 0) return builder.Then(Guid.Empty.ToString(), new object[0]);

                using (var enumerator = events.AsEnumerable().GetEnumerator())
                {
                    enumerator.MoveNext();
                    var continuation = builder.Then(enumerator.Current.Id.ToString(), enumerator.Current);
                    while (enumerator.MoveNext())
                    {
                        continuation = continuation.Then(enumerator.Current.Id.ToString(), enumerator.Current);
                    }
                    return continuation;
                }
            }

            public static IScenarioThenStateBuilder Then(this IScenarioThenStateBuilder builder, params IEvent[] events)
            {
                if (events == null) throw new ArgumentNullException("events");
                return events.Aggregate(builder, (current, @event) => current.Then(@event.Id.ToString(), @event));
            }

            public static void Assert(this IScenarioThenStateBuilder builder)
            {
                //ommitted for brevity.
            }
        }

        public class TodoList : AggregateRootEntity
        {
            public static readonly Func<TodoList> Factory = () => new TodoList();

            TodoList()
            {
                Register<AddedNewTodoList>(When);
                Register<AddedNewItemToTodoList>(When);
                Register<DescribedTodoListItem>(When);
            }

            public TodoList(TodoListId id, string name)
                : this()
            {
                ApplyChange(new AddedNewTodoList(id, name));
            }

            public void AddNewItem(TodoListItemId itemId, string description)
            {
                ApplyChange(new AddedNewItemToTodoList(Id, itemId, description));
            }

            public void DescribeItem(TodoListItemId itemId, string description)
            {
                ApplyChange(new DescribedTodoListItem(itemId, description));
            }

            public TodoListItem FindById(TodoListItemId itemId)
            {
                return _items.Find(item => item.Id.Equals(itemId));
            }

            // state

            public TodoListId Id { get; private set; }
            List<TodoListItem> _items;

            void When(AddedNewTodoList @event)
            {
                Id = new TodoListId(@event.Id);
                _items = new List<TodoListItem>();
            }

            void When(AddedNewItemToTodoList @event)
            {
                var item = new TodoListItem(ApplyChange);
                item.Route(@event);
                _items.Add(item);
            }

            void When(DescribedTodoListItem @event)
            {
                _items.Find(item => item.Id == @event.Id).Route(@event);
            }
        }

        public class TodoListItem : Entity
        {
            public TodoListItem(Action<object> applier)
                : base(applier)
            {
                Register<AddedNewItemToTodoList>(When);
                Register<DescribedTodoListItem>(When);
            }

            public void Describe(string description)
            {
                Apply(new DescribedTodoListItem(Id, description));
            }

            public TodoListItemId Id { get; private set; }

            void When(AddedNewItemToTodoList @event)
            {
                Id = new TodoListItemId(@event.TodoListItemId);
            }

            void When(DescribedTodoListItem @event)
            {
                /* just so you can see we really get to this point */
            }
        }

        public struct TodoListItemId : IEquatable<TodoListItemId>
        {
            readonly Guid _value;

            public TodoListItemId(Guid value)
            {
                _value = value;
            }

            public bool Equals(TodoListItemId other)
            {
                return _value.Equals(other._value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is TodoListItemId && Equals((TodoListItemId) obj);
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }

            public static implicit operator Guid(TodoListItemId id)
            {
                return id._value;
            }
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
            public interface IEvent
            {
                Guid Id { get; }
            }

            public class AddedNewTodoList : IEvent
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

                public readonly string Name;

                public AddedNewTodoList(Guid id, string name)
                {
                    Id = id;
                    Name = name;
                }

                public Guid Id { get; private set; }
            }

            public class AddedNewItemToTodoList : IEvent
            {
                protected bool Equals(AddedNewItemToTodoList other)
                {
                    return Id.Equals(other.Id) && TodoListItemId.Equals(other.TodoListItemId) &&
                           string.Equals(Description, other.Description);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((AddedNewItemToTodoList) obj);
                }

                public override int GetHashCode()
                {
                    return Id.GetHashCode() ^ TodoListItemId.GetHashCode() ^
                           (Description != null ? Description.GetHashCode() : 0);
                }

                public readonly Guid TodoListItemId;
                public readonly string Description;

                public AddedNewItemToTodoList(Guid id, Guid todoListItemId, string description)
                {
                    Id = id;
                    TodoListItemId = todoListItemId;
                    Description = description;
                }

                public Guid Id { get; private set; }
            }

            public class DescribedTodoListItem : IEvent
            {
                protected bool Equals(DescribedTodoListItem other)
                {
                    return string.Equals(Description, other.Description) && Id.Equals(other.Id);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != GetType()) return false;
                    return Equals((DescribedTodoListItem) obj);
                }

                public override int GetHashCode()
                {
                    return (Description != null ? Description.GetHashCode() : 0) ^ Id.GetHashCode();
                }

                public readonly string Description;

                public DescribedTodoListItem(Guid id, string description)
                {
                    Id = id;
                    Description = description;
                }

                public Guid Id { get; private set; }
            }

            public class AddNewTodoList
            {
                public readonly Guid Id;
                public readonly string Name;

                public AddNewTodoList(Guid id, string name)
                {
                    Id = id;
                    Name = name;
                }
            }
        }
    }
}