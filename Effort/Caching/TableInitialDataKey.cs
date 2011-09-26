using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public class TableInitialDataKey : IEquatable<TableInitialDataKey>
    {
        private ConnectionStringKey con;
        private string tableName;

        public TableInitialDataKey(string connectionString, string tableName)
        {
            this.con = new ConnectionStringKey(connectionString);
            this.tableName = tableName ?? string.Empty;
        }

        public bool Equals(TableInitialDataKey other)
        {
            return 
                string.Equals(this.tableName, other.tableName, StringComparison.InvariantCulture) && 
                this.con.Equals(other.con);
        }

        public override bool Equals(object obj)
        {
            TableInitialDataKey other = obj as TableInitialDataKey;

            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.con.GetHashCode() ^ this.tableName.GetHashCode();
        }
    }
}
