using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class TableInformation
    {
        public TableInformation(Type entityType, PropertyInfo[] primaryKeys)
        {
            this.EntityType = entityType;
            this.PrimaryKeys = primaryKeys;
        }

        public Type EntityType { get; private set; }

        public PropertyInfo[] PrimaryKeys { get; private set; }
    }
}
