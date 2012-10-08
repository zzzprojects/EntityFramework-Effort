
namespace Effort.Test.Environment.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Effort.Test.Environment.ResultSets;

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

        public void Assert()
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

            bool result = comparer.Equals(this.Expected, this.Actual);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(result);
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
