// ----------------------------------------------------------------------------------
// <copyright file="DictionaryResultSetElement.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    internal class DictionaryResultSetElement : IResultSetElement
    {
        private IDictionary<string, object> element;

        public DictionaryResultSetElement(IDictionary<string, object> element)
        {
            this.element = element;
        }

        public string[] FieldNames
        {
            get { return this.element.Keys.ToArray(); }
        }

        public object GetValue(string name)
        {
            return this.element[name];
        }

        public bool HasValue(string name)
        {
            return this.element.ContainsKey(name);
        }

        public override string ToString()
        {
            using (TextWriter textWriter = new StringWriter())
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(textWriter, this.element);

                return textWriter.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            IResultSetElement other = obj as IResultSetElement;

            if (other == null)
            {
                return false;
            }

            if (this.FieldNames.Length != other.FieldNames.Length)
            {
                return false;
            }

            for (int i = 0; i < this.FieldNames.Length; i++)
            {
                string name = this.FieldNames[i];
                object value = this.GetValue(name);

                if (!other.HasValue(name))
                {
                    return false;
                }

                object otherValue = other.GetValue(name);

                if (otherValue == null || value == null)
                {
                    return otherValue == null && value == null;
                }

                if (!CompareValues(value, otherValue))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private static bool CompareValues(object x, object y)
        {
            Type type = x.GetType();

            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return NumberComparer(x, y);
            }

            return DefaultComparer(x, y);
        }

        private static bool DefaultComparer(object x, object y)
        {
            return x.Equals(y);
        }

        private static bool NumberComparer(object x, object y)
        {
            decimal xx = Convert.ToDecimal(x);
            decimal yy = Convert.ToDecimal(y);

            xx = Math.Round(yy, 2, MidpointRounding.ToEven);
            yy = Math.Round(yy, 2, MidpointRounding.ToEven);

            return xx == yy;
        }
    }
}
