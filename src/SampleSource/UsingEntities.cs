using System;
using System.Collections.Generic;
using AggregateSource;
using NUnit.Framework;

namespace SampleSource
{
    namespace UsingEntities
    {
        using Messaging;

        [TestFixture]
        public class SampleUsage
        {
            TodoList _list;
            TodoListItemId _todoListItemId;

            [SetUp]
            public void SetUp()
            {
                _list = new TodoList(new TodoListId(Guid.NewGuid()), "Before my 40th birthday");
                _todoListItemId = new TodoListItemId(Guid.NewGuid());
                _list.AddNewItem(_todoListItemId, "Compose a piece of music");
                _list.ClearChanges();
            }

            [Test]
            public void Encapsulated_TodoListItem_Behavior_Changes_Are_Tracked()
            {
                _list.DescribeItem(_todoListItemId, "Compose a crappy piece of music");

                Assert.That(_list.GetChanges(), Is.EquivalentTo(
                    new Object[]
                    {
                        new DescribedTodoListItem(_todoListItemId, "Compose a crappy piece of music")
                    }));
            }

            [Test]
            public void Exposed_TodoListItem_Behavior_Changes_Are_Tracked()
            {
                _list.FindById(_todoListItemId).Describe("Compose a crappy piece of music");

                Assert.That(_list.GetChanges(), Is.EquivalentTo(
                    new Object[]
                    {
                        new DescribedTodoListItem(_todoListItemId, "Compose a crappy piece of music")
                    }));
            }
        }

        public class TodoList : AggregateRootEntity
        {
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

            public class AddedNewItemToTodoList
            {
                protected bool Equals(AddedNewItemToTodoList other)
                {
                    return TodoListId.Equals(other.TodoListId) && TodoListItemId.Equals(other.TodoListItemId) &&
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
                    return TodoListId.GetHashCode() ^ TodoListItemId.GetHashCode() ^
                           (Description != null ? Description.GetHashCode() : 0);
                }

                public readonly Guid TodoListId;
                public readonly Guid TodoListItemId;
                public readonly string Description;

                public AddedNewItemToTodoList(Guid todoListId, Guid todoListItemId, string description)
                {
                    TodoListId = todoListId;
                    TodoListItemId = todoListItemId;
                    Description = description;
                }
            }

            public class DescribedTodoListItem
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

                public readonly Guid Id;
                public readonly string Description;

                public DescribedTodoListItem(Guid id, string description)
                {
                    Id = id;
                    Description = description;
                }
            }
        }
    }
}