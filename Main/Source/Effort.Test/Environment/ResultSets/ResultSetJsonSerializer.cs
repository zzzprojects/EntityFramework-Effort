namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO;

    internal static class ResultSetJsonSerializer
    {
        public static string Serialize(IResultSet resultSet)
        {
            IResultSetElement[] elements = resultSet.Elements.ToArray();
            IDictionary<string, object>[] convertedElements = new IDictionary<string,object>[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                IResultSetElement element = elements[i];

                IDictionary<string, object> convertedElement = new Dictionary<string, object>();

                foreach (string field in element.FieldNames)
                {
                    convertedElement.Add(field, element.GetValue(field));
                }

                convertedElements[i] = convertedElement;
            }


            using (TextWriter textWriter = new StringWriter())
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(textWriter, convertedElements);

                return textWriter.ToString();
            }
        }
    }
}
