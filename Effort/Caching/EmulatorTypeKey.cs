using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Caching
{
    public class EmulatorTypeKey : IEquatable<EmulatorTypeKey>
    {
        private ConnectionStringKey connectionStringKey;
        private string source;
        private bool shared;

        public EmulatorTypeKey(string connectionString, string source, bool shared)
        {
            this.connectionStringKey = new ConnectionStringKey(connectionString);
            this.source = source ?? string.Empty;
            this.shared = shared;
        }

        public bool Equals(EmulatorTypeKey other)
        {
            return
                this.connectionStringKey.Equals(other.connectionStringKey) &&
                this.source.Equals(other.source) &&
                this.shared.Equals(other.shared);
        }

        public override bool Equals(object obj)
        {
            EmulatorTypeKey other = obj as EmulatorTypeKey;

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
                this.source.GetHashCode() ^
                (this.shared ? 1 : 0);
        }
    }
}
