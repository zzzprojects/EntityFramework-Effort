using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform.Join
{
    public class PropertyList
    {
        private object[] values;

        public PropertyList( object[] values )
        {
            this.values = values;
        }

        public override bool Equals( object obj )
        {
            if( obj is PropertyList == false )
                return false;

            var otherList = obj as PropertyList;

            if( this.values.Length != otherList.values.Length )
                return false;

            for( int i = 0 ; i < this.values.Length ; i++ )
            {
                if( this.values[i] == null && otherList.values[i] == null )
                    continue;

                if( this.values[i] == null || otherList.values[i] == null )
                    return false;

                if( this.values[i].Equals( otherList.values[i] ) == false )
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = this.GetHashCodeFor( 0 );

            for( int i = 1 ; i < this.values.Length ; i++ )
            {
                result ^= this.GetHashCodeFor( i );
            }

            return result;
        }

        private int GetHashCodeFor( int index )
        {
            if( this.values[index] == null )
                return 0;
            else
                return this.values[index].GetHashCode();
        }

        /// <summary>
        /// JoinModifier miatt kell, WTF, todo, stb
        /// </summary>
        public object FirstProperty
        {
            get
            {
                return this.values[0];
            }
        }
    }
}
