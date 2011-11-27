using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.DataInitialization;

namespace Effort.CodeFirst
{
	internal class EmptyDataSourceFactory : IDataSourceFactory
	{
		private string connectionString;

		public EmptyDataSourceFactory(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public IDataSource Create(string tableName, Type entityType)
		{
			return new EmptyDataSource(this.connectionString, tableName, entityType);
		}

		public void Dispose()
		{

		}

	}
}
