using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Effort.DatabaseManagement
{
    internal class TableInformation
    {
        public TableInformation(Type entityType, PropertyInfo[] primaryKeys, PropertyInfo identityField)
        {
            this.EntityType = entityType;
            this.PrimaryKeyFields = primaryKeys;
            this.IdentityField = identityField;
        }

        public Type EntityType { get; private set; }

        public PropertyInfo[] PrimaryKeyFields { get; private set; }

        public PropertyInfo IdentityField { get; private set; }
    }
}
