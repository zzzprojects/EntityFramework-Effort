using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Effort.DatabaseManagement;
using MMDB;
using Effort.DatabaseManagement;
using System.Linq.Expressions;
using System.Reflection;

namespace Effort.Configuration
{
	public static class EffortConfigurator
	{
		public static void UpdateStatistics<TObjectSet>(ObjectContext context,
			Expression<Func<TObjectSet>> tableSelector)
		{
			if (context.Connection.State == System.Data.ConnectionState.Closed)
				context.Connection.Open();
			var table = getTable<TObjectSet>(context, tableSelector);

			table.Statistics.UpdateAll();
		}

		private static MMDB.Table.IReflectionTable getTable<TObjectSet>(ObjectContext context, Expression<Func<TObjectSet>> tableSelector)
		{
			var connectionString = context.Connection.ConnectionString;
			var db = DbInstanceStore.GetDbInstance(connectionString, null);
			var tableName = ((MemberExpression)tableSelector.Body).Member.Name;
			var table = db.GetTable(tableName);
			return table;
		}
		public static void UpdateStatistics<TEntity, TColumn>(ObjectContext context,
			Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
				where TEntity : class
		{
			var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
			var column = getColumn<TEntity, TColumn>(columnSelector);

			table.Statistics.Update(column);
		}

		private static MemberInfo getColumn<TEntity, TColumn>(Expression<Func<TEntity, TColumn>> columnSelector)
		{
			MemberExpression expr = columnSelector.Body as MemberExpression;
			if (columnSelector is UnaryExpression)
			{
				expr = (columnSelector as UnaryExpression).Operand as MemberExpression;
			}
			var columnName = expr.Member.Name;
			var columnMember = typeof(TEntity).GetProperty(columnName);

			return columnMember;
		}
		public static void CreateStatistics<TEntity, TColumn>(ObjectContext context,
			Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
			where TEntity : class
		{
			var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
			var column = getColumn<TEntity, TColumn>(columnSelector);

			table.Statistics.CreateIntegerStatistics(column.Name);
		}

		public static void Test<TEntity, TColumn>(ObjectContext context,
			Expression<Func<ObjectSet<TEntity>>> tableSelector, Expression<Func<TEntity, TColumn>> columnSelector)
				where TEntity : class
		{
			var table = getTable<ObjectSet<TEntity>>(context, tableSelector);
			var column = getColumn<TEntity, TColumn>(columnSelector);
		}
	}
}
