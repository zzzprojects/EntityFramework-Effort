using System.Reflection;

namespace Effort.Internal.DbManagement
{
    public class DbRelationInformation
    {
        public DbRelationInformation(string fromTable, PropertyInfo[] fromProperties, string toTable, PropertyInfo[] toProperties)
        {
            this.FromTable = fromTable;
            this.ToTable = toTable;
            this.FromProperties = fromProperties;
            this.ToProperties = toProperties;
        }

        public string FromTable { get; private set; }

        public string ToTable { get; private set; }

        public PropertyInfo[] FromProperties { get; private set; }

        public PropertyInfo[] ToProperties { get; private set; }
    }
}
