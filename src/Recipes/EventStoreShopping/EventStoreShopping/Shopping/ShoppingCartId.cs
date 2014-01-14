using System;

namespace EventStoreShopping.Shopping
{
    public struct ShoppingCartId : IEquatable<ShoppingCartId>
    {
        private readonly string _value;

        public ShoppingCartId(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            _value = value;
        }

        public bool Equals(ShoppingCartId other)
        {
            return string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ShoppingCartId && Equals((ShoppingCartId)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(ShoppingCartId left, ShoppingCartId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ShoppingCartId left, ShoppingCartId right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(ShoppingCartId id)
        {
            return id._value;
        }

        public override string ToString()
        {
            return "ShoppingCart/" + _value;
        }
    }
}