// --------------------------------------------------------------------------------------------
// <copyright file="Correctness.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Test.Internal.Queries
{
    using System.Collections.Generic;
    using Effort.Test.Internal.ResultSets;

    internal class Correctness : ICorrectness
    {
        private IResultSet expected;
        private IResultSet actual;
        private bool ordered;

        public Correctness(IResultSet expected, IResultSet actual)
            : this(expected, actual, false)
        {

        }

        public Correctness(IResultSet expected, IResultSet actual, bool ordered)
        {
            this.expected = expected;
            this.actual = actual;
            this.ordered = ordered;
        }

        public bool Check()
        {
            IEqualityComparer<IResultSet> comparer = null;

            if (this.ordered)
            {
                comparer = new OrderedResultSetComparer();
            }
            else
            {
                comparer = new ResultSetComparer();
            }

            return comparer.Equals(this.Expected, this.Actual);
        }

        public IResultSet Expected
        {
            get { return this.expected; }
        }

        public IResultSet Actual
        {
            get { return this.actual; }
        }
    }
}
