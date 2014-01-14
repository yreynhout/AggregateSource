using System;
using System.Collections.Generic;
using AggregateSource;
using EventStoreShopping.Messaging.Events;

namespace EventStoreShopping.Shopping
{
    public class ShoppingCart : AggregateRootEntity
    {
        public static readonly Func<ShoppingCart> Factory = () => new ShoppingCart();

        private ShoppingCartId _id;
        private bool _checkedOut;
        private Items _items;

        ShoppingCart()
        {
            Register<StartedShopping>(_ =>
            {
                _id = new ShoppingCartId(_.CartId);
                _checkedOut = false;
                _items = new Items();
            });
            Register<CheckedoutCart>(_ => _checkedOut = true);
            Register<AddedItemToCart>(_ => _items.Add(new Item(new ItemId(_.ItemId), _.Count)));
            Register<RemovedItemFromCart>(_ => _items.Remove(new ItemId(_.ItemId)));
            Register<IncrementedItemCountInCart>(_ => _items.Get(new ItemId(_.ItemId)).Increment());
            Register<DecrementedItemCountInCart>(_ => _items.Get(new ItemId(_.ItemId)).Decrement());
        }

        public ShoppingCart(ShoppingCartId id) 
            : this()
        {
            ApplyChange(new StartedShopping { CartId = id });
        }

        public void AddItem(ItemId itemId, int count)
        {
            ThrowIfCheckedOut();
            ApplyChange(new AddedItemToCart { CartId = _id, ItemId = itemId, Count = count});
        }

        public void IncrementItemCount(ItemId itemId)
        {
            ThrowIfCheckedOut();
            ThrowIfItemNotInCart(itemId);
            ApplyChange(new IncrementedItemCountInCart { CartId = _id, ItemId = itemId });
        }

        public void DecrementItemCount(ItemId itemId)
        {
            ThrowIfCheckedOut();
            ThrowIfItemNotInCart(itemId);
            if (_items.Get(itemId).CanDecrement())
                ApplyChange(new DecrementedItemCountInCart { CartId = _id, ItemId = itemId });
        }

        public void RemoveItem(ItemId itemId)
        {
            ThrowIfCheckedOut();
            ThrowIfItemNotInCart(itemId);
            ApplyChange(new RemovedItemFromCart { CartId = _id, ItemId = itemId });
        }

        public void Checkout()
        {
            if (_checkedOut) return;
            ApplyChange(new CheckedoutCart { CartId = _id });
        }

        private void ThrowIfCheckedOut()
        {
            if (_checkedOut)
                throw new ShoppingCartAlreadyCheckedOutException();
        }

        private void ThrowIfItemNotInCart(ItemId itemId)
        {
            if (!_items.Contains(itemId))
                throw new ShoppingCartDoesNotContainItemException();
        }

        class Items
        {
            private readonly List<Item> _items;

            public Items()
            {
                _items = new List<Item>();
            }

            public void Add(Item item)
            {
                _items.Add(item);
            }

            public void Remove(ItemId itemId)
            {
                _items.Remove(_items.Find(_ => _.ItemId == itemId));
            }

            public bool Contains(ItemId itemId)
            {
                return _items.Exists(_ => _.ItemId == itemId);
            }

            public Item Get(ItemId itemId)
            {
                return _items.Find(_ => _.ItemId == itemId);
            }
        }

        class Item
        {
            private readonly ItemId _itemId;
            private int _count;

            public Item(ItemId itemId, int count)
            {
                _itemId = itemId;
                _count = count;
            }

            public ItemId ItemId { get { return _itemId; } }

            public void Increment()
            {
                _count += 1;
            }

            public void Decrement()
            {
                _count -= 1;
            }

            public bool CanDecrement()
            {
                return _count > 0;
            }
        }
    }
}
