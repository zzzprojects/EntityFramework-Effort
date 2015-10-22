// --------------------------------------------------------------------------------------------
// <copyright file="EffortDataReader.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

    /// <summary>
    ///     Reads a forward-only stream of rows from a data source.
    /// </summary>
    public class EffortDataReader : DbDataReader
    {
        private IEnumerator enumerator;
        private int recordsAffected;

        private FieldDescription[] fields;
        private object[] currentValues;

        private DbContainer container;

        internal EffortDataReader(
            IEnumerable result, 
            int recordsAffected,
            FieldDescription[] fields, 
            DbContainer container)
        {
            this.enumerator = result.GetEnumerator();
            this.recordsAffected = recordsAffected;

            this.fields = fields;
            this.container = container;
        }

        /// <summary>
        ///     Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public override int Depth
        {
            get { return 0; }
        }

        /// <summary>
        ///     Gets the number of rows changed, inserted, or deleted by execution of the 
        ///     command.
        /// </summary>
        /// <returns>
        ///     The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0
        ///     if no rows were affected or the statement failed.
        /// </returns>
        public override int RecordsAffected
        {
            get { return this.recordsAffected; }
        }

        /// <summary>
        ///     Gets the number of columns in the current row.
        /// </summary>
        /// <returns>
        ///     The number of columns in the current row.
        /// </returns>
        public override int FieldCount
        {
            get
            {
                if (this.fields == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return this.fields.Length;
                }
            }
        }

        /// <summary>
        ///     Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override bool GetBoolean(int ordinal)
        {
            return (bool)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override byte GetByte(int ordinal)
        {
            return (byte)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Reads a stream of bytes from the specified column, starting at location
        ///     indicated by <paramref name="dataOffset" />, into the buffer, starting at the
        ///     location indicated by <paramref name="bufferOffset" />.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <param name="dataOffset">
        ///     The index within the row from which to begin the read operation.
        /// </param>
        /// <param name="buffer">
        ///     The buffer into which to copy the data.
        /// </param>
        /// <param name="bufferOffset">
        ///     The index with the buffer to which the data will be copied.
        /// </param>
        /// <param name="length">
        ///     The maximum number of characters to read.
        /// </param>
        /// <returns>
        ///     The actual number of bytes read.
        /// </returns>
        public override long GetBytes(
            int ordinal, 
            long dataOffset, 
            byte[] buffer, 
            int bufferOffset, 
            int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the value of the specified column as a single character.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Reads a stream of characters from the specified column, starting at location 
        ///     indicated by <paramref name="dataOffset" />, into the buffer, starting at the 
        ///     location indicated by <paramref name="bufferOffset" />.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <param name="dataOffset">
        ///     The index within the row from which to begin the read operation.
        /// </param>
        /// <param name="buffer">
        ///     The buffer into which to copy the data.
        /// </param>
        /// <param name="bufferOffset">
        ///     The index with the buffer to which the data will be copied.
        /// </param>
        /// <param name="length">
        ///     The maximum number of characters to read.
        /// </param>
        /// <returns>
        ///     The actual number of characters read.
        /// </returns>
        public override long GetChars(
            int ordinal, 
            long dataOffset, 
            char[] buffer, 
            int bufferOffset, 
            int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets name of the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     A string representing the name of the data type.
        /// </returns>
        public override string GetDataTypeName(int ordinal)
        {
            return this.fields[ordinal].Type.Name;
        }

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="T:System.DateTime" /> 
        ///     object.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a <see cref="T:System.Decimal" />
        ///     object.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a double-precision floating point
        ///     number.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override double GetDouble(int ordinal)
        {
            return (double)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The data type of the specified column.
        /// </returns>
        public override Type GetFieldType(int ordinal)
        {
            return this.fields[ordinal].Type;
        }

        /// <summary>
        ///     Gets the value of the specified column as a single-precision floating point
        ///     number.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override float GetFloat(int ordinal)
        {
            return (float)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a globally-unique identifier (GUID).
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override Guid GetGuid(int ordinal)
        {
            return (Guid)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        /// The value of the specified column.
        /// </returns>
        public override short GetInt16(int ordinal)
        {
            return (short)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override int GetInt32(int ordinal)
        {
            return (int)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        ///  </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override long GetInt64(int ordinal)
        {
            return (long)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        ///  </param>
        /// <returns>
        ///     The name of the specified column.
        /// </returns>
        public override string GetName(int ordinal)
        {
            if (this.fields == null)
            {
                throw new InvalidOperationException();
            }

            return this.fields[ordinal].Name;
        }

        /// <summary>
        ///     Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">
        ///     The name of the column.
        /// </param>
        /// <returns>
        ///     The zero-based column ordinal.
        /// </returns>
        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to 
        ///     iterate through the rows in the data reader.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate 
        ///     through the rows in the data reader.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Returns a <see cref="T:System.Data.DataTable" /> that describes the column
        ///     metadata of the <see cref="T:System.Data.Common.DbDataReader" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        public override DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets the value of the specified column as an instance of 
        ///     <see cref="T:System.String" />.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override string GetString(int ordinal)
        {
            return (string)this.GetValue(ordinal);
        }

        /// <summary>
        ///     Gets the value of the specified column as an instance of 
        ///     <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override object GetValue(int ordinal)
        {
            object result = this.currentValues[ordinal];
            Type resultType = this.fields[ordinal].Type;

            result = this.container.TypeConverter.ConvertClrObject(result, resultType);

            if (result == null)
            {
                result = DBNull.Value;
            }

            return result;
        }

        /// <summary>
        ///     Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">
        ///     An array of <see cref="T:System.Object" /> into which to copy the attribute 
        ///     columns.
        /// </param>
        /// <returns>
        ///     The number of instances of <see cref="T:System.Object" /> in the array.
        /// </returns>
        public override int GetValues(object[] values)
        {
            int size = this.fields.Length;

            for (int i = 0; i < size; i++)
            {
                values[i] = GetValue(i);
            }

            return size;
        }

        /// <summary>
        ///     Gets a value that indicates whether this 
        ///     <see cref="T:System.Data.Common.DbDataReader" /> contains one or more rows.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Data.Common.DbDataReader" /> contains one or 
        ///     more rows; otherwise false.
        /// </returns>
        public override bool HasRows
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        ///     Gets a value indicating whether the 
        ///     <see cref="T:System.Data.Common.DbDataReader" /> is closed.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="T:System.Data.Common.DbDataReader" /> is closed; 
        ///     otherwise false.
        /// </returns>
        public override bool IsClosed
        {
            get
            {
                return this.enumerator != null;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the column contains nonexistent or missing 
        ///     values.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        /// </param>
        /// <returns>
        ///     true if the specified column is equivalent to <see cref="T:System.DBNull" />; 
        ///     otherwise false.
        /// </returns>
        public override bool IsDBNull(int ordinal)
        {
            return this.currentValues[ordinal] == null;
        }

        /// <summary>
        ///     Advances the reader to the next result when reading the results of a batch of 
        ///     statements.
        /// </summary>
        /// <returns>
        ///     true if there are more result sets; otherwise false.
        /// </returns>
        public override bool NextResult()
        {
            return this.Read();
        }

        /// <summary>
        ///     Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>
        ///     true if there are more rows; otherwise false.
        /// </returns>
        public override bool Read()
        {
            bool result = this.enumerator.MoveNext();

            if (result)
            {
                this.EnsureValues();
            }

            return result;
        }

        /// <summary>
        ///     Closes the <see cref="EffortDataReader" /> object.
        /// </summary>
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
        ///     Gets the value of the specified column as an instance of 
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="name">
        ///     The name of the column.
        /// </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override object this[string name]
        {
            get
            {
                return this.GetValue(this.GetOrdinal(name));
            }
        }

        /// <summary>
        ///     Gets the value of the specified column as an instance of 
        /// <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="ordinal">
        ///     The zero-based column ordinal.
        ///  </param>
        /// <returns>
        ///     The value of the specified column.
        /// </returns>
        public override object this[int ordinal]
        {
            get 
            {
                return this.GetValue(ordinal);
            }
        }

        /// <summary>
        ///     Releases the managed resources used by the <see cref="EffortDataReader" /> and 
        ///     optionally releases the unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release managed and unmanaged resources; false to release only 
        ///     unmanaged resources.
        /// </param>
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
                PropertyInfo[] props = current
                    .GetType()
                    .GetProperties()
                    .OrderBy(x => names.IndexOf(x.Name))
                    .ToArray();

                this.currentValues = new object[names.Count];

                for (int i = 0; i < names.Count; i++)
                {
                    PropertyInfo property = props[props.Length - names.Count + i];

                    this.currentValues[i] = property.GetValue(current, null);
                }
            }
        }
    }
}
