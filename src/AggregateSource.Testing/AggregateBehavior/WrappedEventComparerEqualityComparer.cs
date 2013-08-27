using System;
using System.Collections.Generic;
using System.Linq;

namespace AggregateSource.Testing.AggregateBehavior
{
    class WrappedEventComparerEqualityComparer : IEqualityComparer<object>
    {
        readonly IEventComparer _comparer;

        public WrappedEventComparerEqualityComparer(IEventComparer comparer)
        {
            if (comparer == null) throw new ArgumentNullException("comparer");
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