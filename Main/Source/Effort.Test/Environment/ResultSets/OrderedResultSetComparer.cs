namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class OrderedResultSetComparer : IEqualityComparer<IResultSet>
    {
        public bool Equals(IResultSet x, IResultSet y)
        {
            IResultSetElement[] xElements = x.Elements.ToArray();
            IResultSetElement[] yElements = y.Elements.ToArray();

            if (xElements.Length != yElements.Length)
            {
                return false;
            }

            int count = xElements.Length;

            for (int i = 0; i < count; i++)
            {
                IResultSetElement xElement = xElements[i];
                IResultSetElement yElement = yElements[i];

                if (!xElement.Equals(yElement))
                {
                    return false;
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
