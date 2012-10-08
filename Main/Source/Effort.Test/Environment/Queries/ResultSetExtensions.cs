namespace Effort.Test.Environment.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Objects;
    using Effort.Test.Environment.ResultSets;

    internal static class ResultSetExtensions
    {
        public static string ConvertToJsonSerialized(this IResultSet resultSet)
        {
            return ResultSetJsonSerializer.Serialize(resultSet);
        }

        public static string ConvertToJsonSerializedCSharpString(this IResultSet resultSet)
        {
            string result = ConvertToJsonSerialized(resultSet);

            return result.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
