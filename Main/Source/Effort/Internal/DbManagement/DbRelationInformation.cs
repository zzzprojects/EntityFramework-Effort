using System.Reflection;
using System.Linq.Expressions;

namespace Effort.Internal.DbManagement
{
    public class DbRelationInformation
    {
        public DbRelationInformation(string fromTable, PropertyInfo[] fromProperties, string toTable, PropertyInfo[] toProperties, object primaryKeyInfo, object foreignKeyInfo)
        {
            this.PrimaryTable = fromTable;
            this.ForeignTable = toTable;
            this.PrimaryProperties = fromProperties;
            this.ForeignProperties = toProperties;
            this.PrimaryKeyInfo = primaryKeyInfo;
            this.ForeignKeyInfo = foreignKeyInfo;
        }

        public string PrimaryTable { get; private set; }

        public string ForeignTable { get; private set; }

        public PropertyInfo[] PrimaryProperties { get; private set; }

        public PropertyInfo[] ForeignProperties { get; private set; }

        //NMemory.IndexesAnonymousTypeKeyInfo<TEntity, TKey> Create<TEntity, TKey>
        public object PrimaryKeyInfo { get; private set; }
        public object ForeignKeyInfo { get; private set; }

        public object ForeignToPrimary{ get; private set; }
        public object PrimaryToForeign { get; private set; }

        //todo: konverziós függvények, keyinfok (mindent ami állapotmentes)
    }
}
