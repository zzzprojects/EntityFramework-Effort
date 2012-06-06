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
using System.Reflection;

namespace Effort.Test.Utils
{
    public class EntityComparer : IEqualityComparer
    {
        private static Dictionary<Type, PropertyInfo[]> properties = new Dictionary<Type,PropertyInfo[]>();

        public EntityComparer()
        {

        }

        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }

            return this.CompareEqualityCore(x, y);
        }

        private bool CompareEqualityCore(object x, object y)
        {
            PropertyInfo[] props;

            if (!properties.TryGetValue(x.GetType(), out props))
            {
                props = x.GetType().GetProperties();
                properties.Add(x.GetType(), props);
            }

            using (EntityReferenceCollection refCol = new EntityReferenceCollection())
            {
                // The comparsion already has began, so skip
                if (refCol.Has(x, y))
                {
                    return true;
                }

                // Register comparsion
                refCol.Add(x, y);



                foreach (var prop in props)
                {
                    Type propType = prop.PropertyType;

                    object xVal = prop.GetValue(x, null);
                    object yVal = prop.GetValue(y, null);

                    //Null check
                    if (xVal == null || yVal == null)
                    {
                        if (xVal == null && yVal == null)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    var comparer = EqualityComparers.Create(propType);

                    if (!comparer.Equals(xVal, yVal))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
