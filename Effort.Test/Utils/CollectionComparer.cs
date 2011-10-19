using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Effort.Test.Utils
{
    public class CollectionComparer<T> : IEqualityComparer<IEnumerable<T>>, IEqualityComparer
    {
        bool order;

        public CollectionComparer(bool order)
        {
            this.order = order;
        }

        public bool Equals(IEnumerable<T> first, IEnumerable<T> second)
        {
            List<T> x = first.ToList();
            List<T> y = second.ToList();

            if (x.Count != y.Count)
            {
                return false;
            }

            if (order)
            {
                return this.StrictCompare(x, y);
            }
            else
            {
                return this.FreeCompare(x, y);
            }
        }

        private bool StrictCompare(List<T> x, List<T> y)
        {
            var comparer = EqualityComparers.Create(typeof(T));

            for (int i = 0; i < x.Count; i++)
            {
                if (!comparer.Equals(x[i], y[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool FreeCompare(List<T> x, List<T> y)
        {
            var comparer = EqualityComparers.Create(typeof(T));

            for (int i = 0; i < x.Count; i++)
            {
                bool found = false;

                for (int j = 0; j < y.Count; j++)
                {
                    if (comparer.Equals(x[i], y[j]))
                    {
                        y.RemoveAt(j);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }


        public int GetHashCode(IEnumerable<T> obj)
        {
            return 0;
        }

        public new bool Equals(object x, object y)
        {
            return this.Equals(
                x as IEnumerable<T>,
                y as IEnumerable<T>);
        }
    }
}
