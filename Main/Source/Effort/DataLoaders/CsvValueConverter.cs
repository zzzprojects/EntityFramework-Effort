// --------------------------------------------------------------------------------------------
// <copyright file="CsvValueConverter.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
// --------------------------------------------------------------------------------------------

namespace Effort.DataLoaders
{
    using System;
    using System.Globalization;
    using System.IO;
    
    /// <summary>
    ///     Converts string values retrieved from Effort compatible CSV files to desired types.
    /// </summary>
    internal class CsvValueConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value to comply with the expected type.
        /// </summary>
        /// <param name="value"> The current value. </param>
        /// <param name="type"> The expected type. </param>
        /// <returns> The expected value. </returns>
        public object ConvertValue(object value, Type type)
        {
            string val = value as string;

            if (type == typeof(string))
            {
                // String handles null values in a separate way
                // null is null, empty is empty
                if (val == null)
                {
                    value = null;
                }
                else
                {
                    value = ResolveEscapeCharacters(val);
                }
            }
            else if (string.IsNullOrEmpty(val))
            {
                // Everything that is empty is null
                value = null;
            }
            else if (type == typeof(byte[]))
            {
                value = Convert.FromBase64String(val);
            }
            else
            {
                // Make the type not nullable
                if (type.IsValueType &&
                    type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }

                if (type == typeof(TimeSpan))
                {
                    value = TimeSpan.Parse(val, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(DateTimeOffset))
                {
                    value = DateTimeOffset.Parse(val, CultureInfo.InvariantCulture);
                }
                else if (type == typeof(Guid))
                {
                    value = Guid.Parse(val);
                }
                else
                {
                    value = Convert.ChangeType(val, type, CultureInfo.InvariantCulture);
                }
            }

            return value;
        }

        private static string ResolveEscapeCharacters(string value)
        {
            char[] chars = value.ToCharArray();

            StringWriter writer = new StringWriter();

            bool escaped = false;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if (escaped)
                {
                    escaped = false;
                    switch (c)
                    {
                        case '\\':
                            writer.Write('\\');
                            break;
                        case 'n':
                            writer.Write('\n');
                            break;
                        case 'r':
                            writer.Write('\r');
                            break;
                        default:
                            throw new FormatException(
                                string.Format(
                                    "\"{0}\" is an invalid string, " +
                                    "it contains an invalid escaped character",
                                    value));
                    }
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else
                {
                    writer.Write(c);
                }
            }

            if (escaped)
            {
                throw new FormatException(
                    string.Format(
                        "\"{0}\" is an invalid string, " +
                        "it contains an invalid escaped character",
                        value));
            }

            return writer.ToString();
        }
    }
}
