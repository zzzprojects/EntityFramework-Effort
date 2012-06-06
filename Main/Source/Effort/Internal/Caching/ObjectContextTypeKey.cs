using System;

namespace Effort.Internal.Caching
{
    internal class ObjectContextTypeKey : IEquatable<ObjectContextTypeKey>
    {
        private string entityConnectionString;
        private string effortConnectionString;
        private string objectContextType;

        public ObjectContextTypeKey(string entityConnectionString, string effortConnectionString, Type objectContextType)
        {
            this.entityConnectionString = entityConnectionString ?? string.Empty;
            this.effortConnectionString = effortConnectionString ?? string.Empty;
            this.objectContextType = objectContextType.FullName;
        }


        public bool Equals(ObjectContextTypeKey other)
        {
            if (other == null)
            {
                return false;
            }

            return this.entityConnectionString.Equals(other.entityConnectionString, StringComparison.InvariantCultureIgnoreCase) &&
                this.effortConnectionString.Equals(other.effortConnectionString, StringComparison.InvariantCultureIgnoreCase) &&
                this.objectContextType.Equals(other.objectContextType, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return 
                this.entityConnectionString.GetHashCode() % 
                this.effortConnectionString.GetHashCode() % 
                this.objectContextType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ObjectContextTypeKey);
        }
    }
}
