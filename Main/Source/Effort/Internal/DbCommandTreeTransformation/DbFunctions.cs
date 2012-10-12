using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.TypeConversion;

namespace Effort.Internal.DbCommandTreeTransformation
{
    internal class DbFunctions
    {
        public static decimal? Abs(decimal? number)
        {
            if (number.HasValue)
                return Math.Abs(number.Value);
            return null;

        }

        public static int IndexOf(string a, string b)
        {
            return a.IndexOf(b) +1;
        }

    }
}
