using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Test.Utils
{
    internal class NumberComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }

            decimal first = this.RoundNumber(x);
            decimal second = this.RoundNumber(y);

            return first.Equals(second);
        }

        private decimal RoundNumber(object value)
        {
            decimal dec = Convert.ToDecimal(value);

            return Math.Round(dec, 2, MidpointRounding.ToEven);
        }

        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
