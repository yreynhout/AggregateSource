using System;
using System.Collections.Generic;
#if !NET20
using System.Linq;
#endif

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
#if NET20
            using (var enumerator = _comparer.Compare(x, y).GetEnumerator()) 
            {
                return !enumerator.MoveNext();
            }
#else
            return !_comparer.Compare(x, y).Any();
#endif
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            throw new NotSupportedException();
        }
    }
}