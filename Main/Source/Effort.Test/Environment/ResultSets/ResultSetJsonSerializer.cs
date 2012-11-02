// ----------------------------------------------------------------------------------
// <copyright file="ResultSetJsonSerializer.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Test.Environment.ResultSets
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

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
