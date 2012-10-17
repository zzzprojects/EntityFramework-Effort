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

        public static string ReverseString(string old)
        {
            return new string(old.ToCharArray().Reverse().ToArray());
        }

        public static string Substring(string data, int? begin,int? length)
        {
            return data.Substring(begin.Value - 1, length.Value );
        }

    }
}
