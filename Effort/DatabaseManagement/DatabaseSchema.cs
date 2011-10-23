using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Effort.DatabaseManagement
{
    internal class DatabaseSchema
    {
        private Dictionary<string, TableInformation> schemaDetails;

        public DatabaseSchema()
        {
            this.schemaDetails = new Dictionary<string, TableInformation>();
        }

        public void Register(string tableName, Type entityType, PropertyInfo[] primaryKeyFields, PropertyInfo identityField)
        {
            this.schemaDetails.Add(tableName, new TableInformation(entityType, primaryKeyFields, identityField));
        }

        public TableInformation GetInformation(string tableName)
        {
            return this.schemaDetails[tableName];
        }

        public string[] GetTableNames()
        {
            return this.schemaDetails.Keys.ToArray();
        }

    }
}
