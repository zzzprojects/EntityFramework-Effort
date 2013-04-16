// --------------------------------------------------------------------------------------------
// <copyright file="DataReaderInspectorDataReader.cs" company="Effort Team">
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

namespace Effort.Test.Internal.DataReaderInspector
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;
    using Effort.Test.Internal.ResultSets;

    internal class DataReaderInspectorDataReader : DbDataReader
    {
        private DbDataReader wrappedDataReader;
        private IResultSetComposer composer;
        private bool commitNext;
        private bool needCommit;

        public DataReaderInspectorDataReader(DbDataReader wrappedDataReader, IResultSetComposer composer)
        {
            this.wrappedDataReader = wrappedDataReader;
            this.composer = composer;
            this.commitNext = false;
            this.needCommit = false;
        }

        public override int Depth
        {
            get 
            { 
                return this.wrappedDataReader.Depth; 
            }
        }

        public override int FieldCount
        {
            get 
            { 
                return this.wrappedDataReader.FieldCount; 
            }
        }

        public override bool HasRows
        {
            get
            {
                return this.wrappedDataReader.HasRows;
            }
        }

        public override bool IsClosed
        {
            get
            {
                return this.wrappedDataReader.IsClosed;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                return this.wrappedDataReader.RecordsAffected;
            }
        }

        public override void Close()
        {
            this.CommitComposer();
            this.wrappedDataReader.Close();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.wrappedDataReader.GetDataTypeName(ordinal);
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.wrappedDataReader.GetFieldType(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override bool GetBoolean(int ordinal)
        {
            return (bool)this.wrappedDataReader.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)this.wrappedDataReader.GetByte(ordinal);
        }

        public override char GetChar(int ordinal)
        {
            return (char)this.GetValue(ordinal);
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

        public override string GetString(int ordinal)
        {
            return (string)this.GetValue(ordinal);
        }

        public override bool IsDBNull(int ordinal)
        {
            bool result = this.wrappedDataReader.IsDBNull(ordinal);

            if (result)
            {
                this.WriteResultSetComposer(ordinal, null);
            }

            return result;
        }

        public override object GetValue(int ordinal)
        {
            object result = this.wrappedDataReader.GetValue(ordinal);
            this.WriteResultSetComposer(ordinal, result);

            return result;
        }

        public override int GetValues(object[] values)
        {
            int count = this.wrappedDataReader.GetValues(values);

            for (int i = 0; i < count; i++)
            {
                this.WriteResultSetComposer(i, values[i]);
            }

            return count;
        }

        public override string GetName(int ordinal)
        {
            return this.wrappedDataReader.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.wrappedDataReader.GetOrdinal(name);
        }

        public override IEnumerator GetEnumerator()
        {
            return ((IEnumerable)this.wrappedDataReader).GetEnumerator();
        }

        public override DataTable GetSchemaTable()
        {
            return this.wrappedDataReader.GetSchemaTable();
        }


        public override bool NextResult()
        {
            return this.wrappedDataReader.NextResult();
        }

        public override bool Read()
        {
            bool result = this.wrappedDataReader.Read();

            if (this.commitNext)
            {
                CommitComposer();
            }

            if (result)
            {
                this.commitNext = true;
            }

            return result;
        }

        public override object this[string name]
        {
            get 
            {
                return this.wrappedDataReader[name];
            }
        }

        public override object this[int ordinal]
        {
            get 
            {
                return this.wrappedDataReader[ordinal];
            }
        }

        private void WriteResultSetComposer(int ordinal, object value)
        {
            string name = this.GetName(ordinal);
            this.composer.SetValue<object>(name, value);

            this.needCommit = true;
        }

        private void CommitComposer()
        {
            if (!this.needCommit)
            {
                return;
            }

            this.composer.Commit();
            this.needCommit = false;
        }
    }
}
