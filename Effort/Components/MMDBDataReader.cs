using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Collections;
using System.Reflection;

namespace MMDB.EntityFrameworkProvider.Components
{
    public class MMDBDataReader : DbDataReader
    {
        private IEnumerable source;
        private IEnumerator enumerator;
        private bool isPrimitive;

        private bool closed = true;

        public string PrimitiveName { get; set; }

        private object[] currentValues;

        public MMDBDataReader(IEnumerable source)
        {
            this.source = source;
            this.enumerator = this.source.GetEnumerator();
            this.closed = false;
        }
        public MMDBDataReader(IEnumerable source, bool isPrimitive)
            : this(source)
        {
            this.isPrimitive = isPrimitive;
        }

        public override void Close()
        {
            this.closed = true;
            this.currentValues = null;

            IDisposable disposableEnumerator = this.enumerator as IDisposable;

            if (disposableEnumerator != null)
            {
                disposableEnumerator.Dispose();
            }
        }

        protected override void Dispose(bool disposing)
        {
            //The Dispose method of the base class invokes Close method
            base.Dispose(disposing);
        }

        public override int Depth
        {
            get { throw new NotImplementedException(); }
        }

        public override int FieldCount
        {
            get
            {
                if (isPrimitive == false)
                {
                    throw new NotImplementedException();
                }
                else
                {
                    return 1;
                }
            }
        }

        public override bool GetBoolean( int ordinal )
        {
            return (bool)this.GetValue( ordinal );
        }

        public override byte GetByte( int ordinal )
        {
            return (byte)this.GetValue( ordinal );
        }

        public override long GetBytes( int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length )
        {
            throw new NotImplementedException();
        }

        public override char GetChar( int ordinal )
        {
            throw new NotImplementedException();
        }

        public override long GetChars( int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length )
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName( int ordinal )
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime( int ordinal )
        {
            return (DateTime)this.GetValue( ordinal );
        }

        public override decimal GetDecimal( int ordinal )
        {
            return (decimal)this.GetValue( ordinal );
        }

        public override double GetDouble( int ordinal )
        {
            return (double)this.GetValue( ordinal );
        }

        public override Type GetFieldType( int ordinal )
        {
            return this.GetValue(ordinal).GetType();
        }

        public override float GetFloat( int ordinal )
        {
            return (float)this.GetValue( ordinal );
        }

        public override Guid GetGuid( int ordinal )
        {
            return (Guid)this.GetValue(ordinal);
        }

        public override short GetInt16( int ordinal )
        {
            return (short)this.GetValue( ordinal );
        }

        public override int GetInt32( int ordinal )
        {
            return (int)this.GetValue( ordinal );
        }

        public override long GetInt64( int ordinal )
        {
            return (long)this.GetValue( ordinal );
        }

        public override string GetName( int ordinal )
        {
            return this.PrimitiveName;
        }

        public override int GetOrdinal( string name )
        {
            throw new NotImplementedException();
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override System.Data.DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public override string GetString( int ordinal )
        {
            // Ezt meg ki kell tapasztalni, hogyan mukodjon
            // amikor maga a bejart elem primitiv akkor is ez hivodik
            if (this.enumerator.Current is string)
            {
                return (string)this.enumerator.Current;
            }

            return (string)this.GetValue( ordinal );
        }

        public override object GetValue( int ordinal )
        {
            object result = null;

            if (this.enumerator.Current.GetType().IsPrimitive)
            {
                result = this.enumerator.Current;
            }
            else
            {
                result = this.currentValues[ordinal];
            }

            return result;
        }

        public override int GetValues( object[] values )
        {
            throw new NotImplementedException();
        }

        public override bool HasRows
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsClosed
        {
            get { return this.closed; }
        }

        public override bool IsDBNull( int ordinal )
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

        private void EnsureValues()
        {
            object current = this.enumerator.Current;
            PropertyInfo[] props = current.GetType().GetProperties();

            this.currentValues = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                this.currentValues[i] = props[i].GetValue(current, null);
            }
        }
    }
}
