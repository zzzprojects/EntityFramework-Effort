using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Caching
{
    internal class ObjectContextTypeKey : IEquatable<ObjectContextTypeKey>
    {
        private ConnectionStringKey connectionStringKey;
        private string source;
        private bool shared;
        private string contextType;

        public ObjectContextTypeKey(string connectionString, Type contextType, string source, bool shared)
        {
            this.connectionStringKey = new ConnectionStringKey(connectionString);
            this.source = source ?? string.Empty;
            this.shared = shared;
            this.contextType = contextType.FullName;
        }

        public bool Equals(ObjectContextTypeKey other)
        {
            return
                this.connectionStringKey.Equals(other.connectionStringKey) &&
                this.contextType.Equals(other.contextType) &&
                this.source.Equals(other.source) &&
                this.shared.Equals(other.shared);
        }

        public override bool Equals(object obj)
        {
            ObjectContextTypeKey other = obj as ObjectContextTypeKey;

            if (other == null)
            {
                return false;
            }

            return this.Equals(obj);
        }

        public override int GetHashCode()
        {
            return
                this.connectionStringKey.GetHashCode() ^
                this.contextType.GetHashCode() ^
                this.source.GetHashCode() ^
                (this.shared ? 1 : 0);
        }
    }
}
