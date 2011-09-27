using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Caching
{
    internal class ConnectionStringKey : IEquatable<ConnectionStringKey>
    {
        private string connectionString;

        public ConnectionStringKey(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ConnectionStringKey);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public bool Equals(ConnectionStringKey other)
        {
            if (other == null)
            {
                return false;
            }

            return ConnectionStringHelper.HasIdenticalAttributes(other.connectionString, this.connectionString);
        }
    }
}
