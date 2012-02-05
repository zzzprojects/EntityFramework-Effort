#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

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
