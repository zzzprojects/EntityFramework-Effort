namespace Effort.Test.Tools.DataReaderInspector
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.Common;

    internal class DataReaderInspectorDataReader : DbDataReader
    {
        private DbDataReader wrappedDataReader;
        private IResultSetComposer composer;
        private bool commitNext;

        public DataReaderInspectorDataReader(DbDataReader wrappedDataReader, IResultSetComposer composer)
        {
            this.wrappedDataReader = wrappedDataReader;
            this.composer = composer;
            this.commitNext = false;
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
                this.composer.SetValue<object>(GetName(ordinal), DBNull.Value);
            }

            return result;
        }

        public override object GetValue(int ordinal)
        {
            object result = this.wrappedDataReader.GetValue(ordinal);

            this.composer.SetValue<object>(GetName(ordinal), result);

            return result;
        }

        public override int GetValues(object[] values)
        {
            return this.wrappedDataReader.GetValues(values);
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
                this.composer.Commit();
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
    }
}
