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

namespace Effort.Test.Utils
{
    public class EntityReferenceCollection : IDisposable
    {
        [ThreadStatic]
        private static int counter;
        [ThreadStatic]
        private static List<EntityComparsion> comparsions;

        [ThreadStatic]
        private static Stack<List<EntityComparsion>> comparsionsStack;

        public EntityReferenceCollection()
        {
            if (comparsionsStack == null)
            {
                comparsionsStack = new Stack<List<EntityComparsion>>();
            }

            if (counter == 0)
            {
                comparsions = new List<EntityComparsion>();
                comparsions = new List<EntityComparsion>();
            }
            else
            {
                comparsionsStack.Push(comparsions);

                comparsions = comparsions.ToList();
            }

            counter++;
        }


        public void Add(object x, object y)
        {
            comparsions.Add(new EntityComparsion(x, y));
        }

        public bool Has(object x, object y)
        {
            for (int i = 0; i < comparsions.Count; i++)
            {
                if (comparsions[i].Equals(x, y))
                {
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            if (counter > 1)
            {
                comparsions = comparsionsStack.Pop();
            }

            counter--;
        }
    }

    public class EntityComparsion
    {
        private object x;
        private object y;

        public EntityComparsion(object x, object y)
        {
            this.x = x;
            this.y = y;
        }

        public new bool Equals(object x, object y)
        {
            return object.ReferenceEquals(this.x, x) && object.ReferenceEquals(this.y, y);
        }
    }
}
