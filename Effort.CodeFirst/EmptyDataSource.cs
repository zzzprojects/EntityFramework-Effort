using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.DataInitialization;

namespace Effort.CodeFirst
{
	internal class EmptyDataSource : IDataSource
	{
		private string connectionString;
		private string tableName;

		public EmptyDataSource(string connectionString, string tableName, Type entityType)
		{
			this.connectionString = connectionString;
			this.tableName = tableName;
		}

		public IEnumerable<object> GetInitialRecords()
		{
			return Enumerable.Empty<object>();
		}
	}
}
