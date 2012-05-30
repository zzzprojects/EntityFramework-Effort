using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Collections;
using System.Reflection;
using System.Data;
using Effort.Internal.DbManagement;

namespace Effort.Provider
{
    public class EffortDataReader : DbDataReader
    {
        private IEnumerator enumerator;

        private string[] fieldNames;
        private object[] currentValues;

        private DbContainer dbContainer;

        internal EffortDataReader(IEnumerable result, string[] fieldNames, DbContainer dbContainer)
        {
            this.fieldNames = fieldNames;
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
                if (fieldNames == null)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    return fieldNames.Length;
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
            if (fieldNames == null)
            {
                throw new NotSupportedException();
            }

            return fieldNames[ordinal];
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

            // TODO: project
            result = this.dbContainer.TypeConverter.ConvertClrValueFromClrValue(result);

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
            List<string> names = this.fieldNames.ToList();

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
