// ----------------------------------------------------------------------------------
// <copyright file="EffortDataReader.cs" company="Effort Team">
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

namespace Effort.Provider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.DbManagement;

    public class EffortDataReader : DbDataReader
    {
        private IEnumerator enumerator;

        private FieldDescription[] fields;
        private object[] currentValues;

        private DbContainer dbContainer;

        internal EffortDataReader(IEnumerable result, FieldDescription[] fields, DbContainer dbContainer)
        {
            this.fields = fields;
            this.enumerator = result.GetEnumerator();

            this.dbContainer = dbContainer;
        }

        public override void Close()
        {
            IDisposable disposeableEnumerator = this.enumerator as IDisposable;

            if (disposeableEnumerator != null)
            {
                disposeableEnumerator.Dispose();
            }

            this.enumerator = null;
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public override int Depth
        {
            get { return 0; }
        }

        public override int FieldCount
        {
            get
            {
                if (fields == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return fields.Length;
                }
            }
        }

        public override bool GetBoolean(int ordinal)
        {
            return (bool)this.GetValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)this.GetValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)this.GetValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)this.GetValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)this.GetValue(ordinal);
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.GetValue(ordinal).GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return (float)this.GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)this.GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)this.GetValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)this.GetValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)this.GetValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            if (this.fields == null)
            {
                throw new InvalidOperationException();
            }

            return this.fields[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            return (string)this.GetValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            object result = this.currentValues[ordinal];

            result = this.dbContainer.TypeConverter.ConvertClrObject(result, this.fields[ordinal].Type);

            if (result == null)
            {
                result = DBNull.Value;
            }

            return result;
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool HasRows
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsClosed
        {
            get
            {
                return this.enumerator != null;
            }
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.currentValues[ordinal] == null;
        }

        public override bool NextResult()
        {
            return this.Read();
        }

        public override bool Read()
        {
            bool result = this.enumerator.MoveNext();

            if (result)
            {
                // currentValues feltöltése
                this.EnsureValues();
            }

            return result;
        }

        public override int RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[int ordinal]
        {
            get { throw new NotImplementedException(); }
        }


        protected override void Dispose(bool disposing)
        {
            // The Dispose method of the base class invokes Close method
            base.Dispose(disposing);
        }

        private void EnsureValues()
        {
            object current = this.enumerator.Current;
            List<string> names = this.fields.Select(f => f.Name).ToList();

            if (current is Dictionary<string, object>)
            {
                var dict = current as Dictionary<string, object>;

                this.currentValues = new object[names.Count];

                foreach (var item in dict)
                {
                    int index = names.IndexOf(item.Key);
                    this.currentValues[index] = item.Value;
                }
            }
            else
            {
                PropertyInfo[] props = current.GetType().GetProperties().OrderBy(x => names.IndexOf(x.Name)).ToArray();

                this.currentValues = new object[names.Count];

                for (int i = 0; i < names.Count; i++)
                {
                    this.currentValues[i] = props[props.Length - names.Count + i].GetValue(current, null);
                }
            }
        }
    }
}
