namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

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
