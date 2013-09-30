using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing
{
    class WrappedEventComparerEqualityComparer : IEqualityComparer<object>
    {
        readonly IEventComparer _comparer;

        public WrappedEventComparerEqualityComparer(IEventComparer comparer)
        {
            _comparer = comparer;
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            return !_comparer.Compare(x, y).Any();
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotSupportedException();
        }
    }
}