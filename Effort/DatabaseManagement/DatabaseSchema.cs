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
		private Dictionary<Type, TableInformation> schemaDetailsByTypeName;

		public DatabaseSchema()
		{
			this.schemaDetails = new Dictionary<string, TableInformation>();
			this.schemaDetailsByTypeName = new Dictionary<Type, TableInformation>();
		}

		public void Register(string tableName, Type entityType, PropertyInfo[] primaryKeyFields, PropertyInfo identityField,
			PropertyInfo[] properties)
		{
			var info = new TableInformation(entityType, primaryKeyFields, identityField, properties);
			this.schemaDetails.Add(tableName, info);
			this.schemaDetailsByTypeName.Add(entityType, info);
		}

		public TableInformation GetInformation(string tableName)
		{
			return this.schemaDetails[tableName];
		}

		public TableInformation GetInformationByTypeName(Type entityType )
		{
			TableInformation info;

			if (this.schemaDetailsByTypeName.TryGetValue(entityType, out info))
				return info;
			else
				return null;
		}

		public string[] GetTableNames()
		{
			return this.schemaDetails.Keys.ToArray();
		}

	}
}
