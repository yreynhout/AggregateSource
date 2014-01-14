using System;

namespace EventStoreShopping.Shopping
{
    public struct ItemId : IEquatable<ItemId>
    {
        private readonly string _value;

        public ItemId(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _value = value;
        }

        public bool Equals(ItemId other)
        {
            return string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ItemId && Equals((ItemId)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(ItemId left, ItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemId left, ItemId right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(ItemId id)
        {
            return id._value;
        }

        public override string ToString()
        {
            return "Item/" + _value;
        }
    }
}