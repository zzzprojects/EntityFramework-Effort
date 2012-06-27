using System.Reflection;

namespace Effort.Internal.DbManagement
{
    public class DbRelationInformation
    {
        public DbRelationInformation(string fromTable, PropertyInfo[] fromProperties, string toTable, PropertyInfo[] toProperties)
        {
            this.PrimaryTable = fromTable;
            this.ForeignTable = toTable;
            this.PrimaryProperties = fromProperties;
            this.ForeignProperties = toProperties;
        }

        public string PrimaryTable { get; private set; }

        public string ForeignTable { get; private set; }

        public PropertyInfo[] PrimaryProperties { get; private set; }

        public PropertyInfo[] ForeignProperties { get; private set; }
    }
}
