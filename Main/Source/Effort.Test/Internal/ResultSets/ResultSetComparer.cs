// --------------------------------------------------------------------------------------------
// <copyright file="ResultSetComparer.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.ResultSets
{
    using System.Collections.Generic;
    using System.Linq;

    internal class ResultSetComparer : IEqualityComparer<IResultSet>
    {
        public bool Equals(IResultSet x, IResultSet y)
        {
            List<IResultSetElement> xElements = x.Elements.ToList();
            List<IResultSetElement> yElements = y.Elements.ToList();

            if (xElements.Count != yElements.Count)
            {
                return false;
            }

            for (int i = 0; i < xElements.Count; i++)
            {
                IResultSetElement xElement = xElements[i];

                bool equals = false;

                int j = 0;
                for (; j < yElements.Count; j++)
                {
                    IResultSetElement yElement = yElements[j];

                    if (xElement.Equals(yElement))
                    {
                        equals = true;
                        break;
                    }
                }

                if (!equals)
                {
                    return false;
                }
                else
                {
                    yElements.RemoveAt(j);
                }
            }

            return true;
        }


        public int GetHashCode(IResultSet obj)
        {
            return 0;
        }
    }
}
