namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
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
