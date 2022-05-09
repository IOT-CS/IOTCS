using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Core.Collections
{
    public interface IConcurrentList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        int BinSearch(T value);

        T Find(Predicate<T> match);

        bool Exists(Predicate<T> match);
    }
}
