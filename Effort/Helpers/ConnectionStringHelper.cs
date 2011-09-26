using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace MMDB.EntityFrameworkProvider
{
    internal class ConnectionStringHelper
    {
        public static T GetValue<T>(string connectionString, string field)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (!builder.ContainsKey(field))
            {
                return default(T);
            }

            string value = builder[field] as string;

            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static bool HasIdenticalAttributes(string compare, string compareTo)
        {
            DbConnectionStringBuilder bCompare = new DbConnectionStringBuilder();
            bCompare.ConnectionString = RemoveWrappedProviderAttribute(compare);

            DbConnectionStringBuilder bCompareTo = new DbConnectionStringBuilder();
            bCompareTo.ConnectionString = RemoveWrappedProviderAttribute(compareTo);

            foreach (string key in bCompareTo.Keys)
            {
                if (bCompare.ContainsKey(key) &&
                    !string.Equals(bCompare[key].ToString(), bCompareTo[key].ToString(), StringComparison.InvariantCulture))
                {
                    return false;
                }
            }

            return true;
        }

        private static string RemoveWrappedProviderAttribute(string connectionString)
        {
            while (connectionString.StartsWith("wrappedprovider", StringComparison.InvariantCultureIgnoreCase))
            {
                int startIndex = connectionString.IndexOf(';') + 1;

                connectionString = connectionString.Substring(startIndex, connectionString.Length - startIndex);
            }

            return connectionString;
        }
    }
}
