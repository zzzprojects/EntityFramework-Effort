using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class DatabaseSchema
    {
        private Dictionary<string, TableInformation> schemaDetails;

        public DatabaseSchema()
        {
            this.schemaDetails = new Dictionary<string, TableInformation>();
        }

        public void Register(string tableName, Type entityType, PropertyInfo[] primaryKeys)
        {
            this.schemaDetails.Add(tableName, new TableInformation(entityType, primaryKeys));
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
