#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.DatabaseManagement;
using Effort.DbCommandTreeTransform;
using Effort.DbCommandTreeTransform.PostProcessing;
using Effort.Helpers;
using EFProviderWrapperToolkit;
using MMDB;
using MMDB.Linq.Visitors;
using MMDB.Logging;
using MMDB.Table;

namespace Effort.Components
{
	public class EffortWrapperCommand : DbCommandWrapper
	{
		public EffortWrapperCommand(DbCommand wrappedCommand, DbCommandDefinitionWrapper commandDefinition)
			: base(wrappedCommand, commandDefinition)
		{

		}

		public Database DatabaseCache
		{
			get
			{
				return this.WrapperConnection.DatabaseCache;
			}
		}

		public EffortWrapperConnection WrapperConnection
		{
			get
			{
				EffortWrapperConnection connection = base.Connection as EffortWrapperConnection;

				if (connection == null)
				{
					throw new InvalidOperationException();
				}

				return connection;
			}
		}

		#region Execute methods

		public override int ExecuteNonQuery()
		{
			this.DatabaseCache.LoggingPort.Send(new StandardMMDBMessage(this.CommandText));

			if (this.Definition.CommandTree is DbUpdateCommandTree)
			{
				return this.PerformUpdate();
			}
			else if (this.Definition.CommandTree is DbDeleteCommandTree)
			{
				return this.PerformDelete();
			}
			else if (this.Definition.CommandTree is DbInsertCommandTree)
			{
				return this.PerformInsert();
			}

			throw new NotSupportedException();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			this.DatabaseCache.LoggingPort.Send(new StandardMMDBMessage(this.CommandText));

			if (this.DesignMode)
			{
				return base.ExecuteDbDataReader(behavior);
			}

			if (this.Definition.CommandTree is DbInsertCommandTree)
			{
				return PerformInsert(behavior);
			}
			else if (base.Definition.CommandTree is DbQueryCommandTree)
			{
				return PerformQuery(behavior);
			}

			throw new NotSupportedException();
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return null;
			}
			set
			{

			}
		}


		#endregion

		#region Query

		private DbDataReader PerformQuery(CommandBehavior behavior)
		{
			DbQueryCommandTree commandTree = base.Definition.CommandTree as DbQueryCommandTree;

			Stopwatch stopWatch = Stopwatch.StartNew();


			// Find tables
			TableScanVisitor tableScanVisitor = new TableScanVisitor();
			tableScanVisitor.Visit(commandTree.Query);

			// CodeFirst-hoz kell
			if (tableScanVisitor.Tables.Contains("EdmMetadata"))
			{
				return new EffortDataReader(new List<object>(), this.getSchema());
			}

			#region Skip Cache

			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
			{
				if (this.WrapperConnection.defaultCacheMode == false || this.WrapperConnection.nonCached.Any())
				{
					//    TableScanVisitor tableScanVisitor = new TableScanVisitor();
					//    tableScanVisitor.Visit( commandTree.Query );

					bool skipCache = false;
					if (this.WrapperConnection.defaultCacheMode && tableScanVisitor.Tables.Any(t => this.WrapperConnection.nonCached.Contains(t)))
						skipCache = true;
					if (this.WrapperConnection.defaultCacheMode == false && tableScanVisitor.Tables.Any(t => this.WrapperConnection.cached.Contains(t)) == false)
						skipCache = true;

					if (skipCache)
					{
						// Not using MMDB Cache
						this.DatabaseCache.LoggingPort.Send(new StandardMMDBMessage("Not using MMDB Cache for the current query."));
						this.EnsureOpenConnection();
						return base.WrappedCommand.ExecuteReader(behavior);
					}
				}
			}

			#endregion

			// Setup expression tranformer
			DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor();
			visitor.SetParameters(commandTree.Parameters.ToArray());
			visitor.TableProvider = new DatabaseWrapper(this.DatabaseCache);

			Expression linqExpression = null;
			Func<Dictionary<string, object>, IEnumerable> storedProc = null;

			bool hasParameters = this.Parameters.Count > 0;
			bool cached = false;

			#region Transform Cache
			if (hasParameters == false &&
				this.DatabaseCache.TransformCache.TryGetValue(this.CommandText, out linqExpression))
			{
				cached = true;
			}
			if (hasParameters && this.DatabaseCache.TransformCacheProcedures.TryGetValue(this.CommandText, out storedProc))
			{
				cached = true;
			}
			if (cached && this.DatabaseCache.LoggingPort != null)
			{
				this.DatabaseCache.LoggingPort.Send(new StandardMMDBMessage("TransformCache used."));
			}

			#endregion

			// Check if this expression tree has not been cached yet
			if (cached == false)
			{
				// Convert DbCommandTree to linq Expression Tree
				linqExpression = visitor.Visit(commandTree.Query);
			
				Console.WriteLine();
				Console.WriteLine("Original expression tree:");
				Console.WriteLine(new ExpressionFormatter().Format(linqExpression, 299));
				Console.WriteLine();

				#region Postprocessing

				//Convert ' Nullable<?> Enumerable.Sum ' to ' Nullable<?> EnumerableNullSum.Sum '
				SumTransformerVisitor sumTransformer = new SumTransformerVisitor();
				linqExpression = sumTransformer.Visit(linqExpression);

				//Clean ' new SingleResult<>(x).FirstOrDefault() '
				ExcrescentSingleResultCleanserVisitor cleanser1 = new ExcrescentSingleResultCleanserVisitor();
				linqExpression = cleanser1.Visit(linqExpression);

				//Clean ' new AT(x).YProp '    (x is the init value of YProp)
				ExcrescentInitializationCleanserVisitor cleanser2 = new ExcrescentInitializationCleanserVisitor();
				linqExpression = cleanser2.Visit(linqExpression);

				#endregion

				// Place the transformed expression into the cache
				if (hasParameters)
				{
					storedProc = this.CreateStoredProcedure(linqExpression);

					this.DatabaseCache.TransformCacheProcedures[this.CommandText] = storedProc;
				}
				else
				{
					this.DatabaseCache.TransformCache[this.CommandText] = linqExpression;
				}
			}

			if (this.DatabaseCache.LoggingPort != null)
			{
				this.DatabaseCache.LoggingPort.Send(
					new StandardMMDBMessage(
						"DbCommandTree converted in {0:0.00} ms",
						stopWatch.Elapsed.TotalMilliseconds));

			}

			if (hasParameters)
			{
				Dictionary<string, object> parameters = this
					 .Parameters
					 .Cast<DbParameter>()
					 .ToDictionary(p => p.ParameterName, p => p.Value as object);

				IEnumerable result = storedProc(parameters);

				return new EffortDataReader(storedProc(parameters), this.getSchema());
			}
			else
			{
				IQueryable result = this.CreateQuery(linqExpression);

				return new EffortDataReader(result, this.getSchema());
			}
		}

		#endregion

		#region Update

		private DatabaseSchema getSchema()
		{
			return this.WrapperConnection.GetDatabaseSchema();
		}

		private int PerformUpdate()
		{
			this.EnsureOpenConnection();

			//Execute update on the database
			int? realDbRowCount = null;

			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
			{
				realDbRowCount = base.WrappedCommand.ExecuteNonQuery();
			}

			DbUpdateCommandTree commandTree = base.Definition.CommandTree as DbUpdateCommandTree;

			IReflectionTable table = null;

			Expression linqExpression = this.GetEnumeratorExpression(commandTree.Predicate, commandTree, out table);
			IQueryable entitiesToUpdate = this.CreateQuery(linqExpression);

			Type type = TypeHelper.GetElementType(table.GetType());

			//Collect the SetClause DbExpressions into a dictionary
			IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions(commandTree.SetClauses);

			//Collection for collection member bindings
			IList<MemberBinding> memberBindings = new List<MemberBinding>();

			DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

			// Setup context for the predicate
			ParameterExpression context = Expression.Parameter(type, "context");
			using (transform.CreateVariable(context, commandTree.Target.VariableName))
			{
				//Initialize member bindings
				foreach (PropertyInfo property in type.GetProperties())
				{
					Expression setter = null;

					// Check if member has set clause
					if (setClauses.ContainsKey(property.Name))
					{
						// Set the clause
						setter = transform.Visit(setClauses[property.Name]);
					}
					else
					{
						// Copy member
						setter = Expression.Property(context, property);
					}


					// If setter was found, insert it
					if (setter != null)
					{
						// Type correction
						setter = ExpressionHelper.CorrectType(setter, property.PropertyType);

						memberBindings.Add(Expression.Bind(property, setter));
					}
				}
			}

			Expression updater =
				Expression.Lambda(
					Expression.MemberInit(Expression.New(type), memberBindings),
					context);

			int rowCount = DatabaseReflectionHelper.UpdateEntities(entitiesToUpdate, updater);

			// Compare the row count in accelerator mode 
			if (realDbRowCount.HasValue && rowCount != realDbRowCount.Value)
			{
				throw new InvalidOperationException();
			}

			return rowCount;
		}

		#endregion

		#region Delete

		private int PerformDelete()
		{
			this.EnsureOpenConnection();

			int? realDbRowCount = null;

			//Execute delete on the database
			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
			{
				realDbRowCount = base.ExecuteNonQuery();
			}

			DbDeleteCommandTree commandTree = base.Definition.CommandTree as DbDeleteCommandTree;

			IReflectionTable table = null;

			Expression linqExpression = this.GetEnumeratorExpression(commandTree.Predicate, commandTree, out table);
			IQueryable entitiesToDelete = this.CreateQuery(linqExpression);

			int rowCount = DatabaseReflectionHelper.DeleteEntities(entitiesToDelete);

			// Compare the row count in accelerator mode 
			if (realDbRowCount.HasValue && rowCount != realDbRowCount.Value)
			{
				throw new InvalidOperationException();
			}

			return rowCount;
		}

		#endregion

		#region Insert

		private int PerformInsert()
		{
			this.EnsureOpenConnection();

			if (WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
			{
				base.WrappedCommand.ExecuteNonQuery();
			}

			DbInsertCommandTree commandTree = base.Definition.CommandTree as DbInsertCommandTree;
			// Get the source table
			IReflectionTable table = this.GetTable(commandTree);

			// Collect the SetClause DbExpressions into a dictionary
			IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions(commandTree.SetClauses);

			// Collection for collection member bindings
			IList<MemberBinding> memberBindings = new List<MemberBinding>();
			DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

			// Initialize member bindings
			foreach (PropertyInfo property in table.EntityType.GetProperties())
			{
				Expression setter = null;

				//Check if member has set clause
				if (setClauses.ContainsKey(property.Name))
				{
					setter = transform.Visit(setClauses[property.Name]);
				}

				// If setter was found, insert it
				if (setter != null)
				{
					// Type correction
					setter = ExpressionHelper.CorrectType(setter, property.PropertyType);

					memberBindings.Add(Expression.Bind(property, setter));
				}
			}

			this.CreateAndInsertEntity(table, memberBindings);

			return 1;
		}

		private DbDataReader PerformInsert(CommandBehavior behavior)
		{
			this.EnsureOpenConnection();

			DbInsertCommandTree commandTree = base.Definition.CommandTree as DbInsertCommandTree;
			Dictionary<string, object> returnedValues = new Dictionary<string, object>();

			// Find the returning properties
			DbNewInstanceExpression returnExpression = commandTree.Returning as DbNewInstanceExpression;

			if (returnExpression == null)
			{
				throw new NotSupportedException("The type of the Returning properties is not DbNewInstanceExpression");
			}

			// Add the returning property names
			foreach (DbPropertyExpression propertyExpression in returnExpression.Arguments)
			{
				returnedValues.Add(propertyExpression.Property.Name, null);
			}

			// In accelerator mode, execute the wrapped command, and marshal the returned values
			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
			{
				DbDataReader reader = base.WrappedCommand.ExecuteReader(behavior);

				returnedValues = new Dictionary<string, object>();

				using (reader)
				{
					if (!reader.Read())
					{
						throw new InvalidOperationException();
					}

					for (int i = 0; i < reader.FieldCount; i++)
					{
						object value = reader.GetValue(i);

						if (!(value is DBNull))
						{
							returnedValues[reader.GetName(i)] = value;
						}
					}
				}
			}


			IReflectionTable table = this.GetTable(commandTree);

			// Collect the SetClause DbExpressions into a dictionary
			IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions(commandTree.SetClauses);

			// Collection for collection member bindings
			IList<MemberBinding> memberBindings = new List<MemberBinding>();
			DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

			// Initialize member bindings
			foreach (PropertyInfo property in table.EntityType.GetProperties())
			{
				Expression setter = null;

				// Check if member has set clause
				if (setClauses.ContainsKey(property.Name))
				{
					setter = transform.Visit(setClauses[property.Name]);

				}
				else if (returnedValues.ContainsKey(property.Name))
				{
					// In accelerator mode, the missing value is filled with the returned value
					if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator)
					{
						setter = Expression.Constant(returnedValues[property.Name]);
					}
				}


				// If setter was found, insert it
				if (setter != null)
				{
					// Type correction
					setter = ExpressionHelper.CorrectType(setter, property.PropertyType);

					memberBindings.Add(Expression.Bind(property, setter));
				}
			}

			object entity = CreateAndInsertEntity(table, memberBindings);

			string[] returnedFields = returnedValues.Keys.ToArray();

			// In emulator mode, the generated values are gathered from the MMDB
			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseEmulator)
			{
				for (int i = 0; i < returnedFields.Length; i++)
				{
					string property = returnedFields[i];

					object value = entity.GetType().GetProperty(property).GetValue(entity, null);

					returnedValues[property] = value;
				}
			}

			return new EffortDataReader(new[] { returnedValues }, returnedFields, this.getSchema());
		}

		private IReflectionTable GetTable(DbInsertCommandTree commandTree)
		{
			DbScanExpression source = commandTree.Target.Expression as DbScanExpression;

			if (source == null)
			{
				throw new InvalidOperationException("The type of the Target property is not DbScanExpression");
			}

			return this.DatabaseCache.GetTable(source.Target.Name);
		}

		private object CreateAndInsertEntity(IReflectionTable table, IList<MemberBinding> memberBindings)
		{
			LambdaExpression expression =
			   Expression.Lambda(
				   Expression.MemberInit(
					   Expression.New(table.EntityType),
					   memberBindings));

			Delegate factory = expression.Compile();

			object newEntity = factory.DynamicInvoke();

			table.Insert(newEntity);

			return newEntity;
		}

		#endregion

		#region Helper methods

		private void EnsureOpenConnection()
		{
			// In accelerator mode, need to deal with wrapped connection
			if (this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator &&
				this.WrappedCommand.Connection.State == ConnectionState.Closed)
			{
				base.WrappedCommand.Connection.Open();

				// Check for an ambient transaction
				var transaction = System.Transactions.Transaction.Current;

				// If DbTransaction is used, then the Transaction.Current is null
				if (transaction != null)
				{
					this.WrappedCommand.Connection.EnlistTransaction(transaction);
				}
			}
		}


		private IDictionary<string, DbExpression> GetSetClauseExpressions(IList<DbModificationClause> clauses)
		{
			IDictionary<string, DbExpression> result = new Dictionary<string, DbExpression>();

			foreach (DbSetClause setClause in clauses.Cast<DbSetClause>())
			{
				DbPropertyExpression property = setClause.Property as DbPropertyExpression;

				if (property == null)
				{
					throw new NotSupportedException(setClause.Property.ExpressionKind.ToString() + " is not supported");
				}

				result.Add(property.Property.Name, setClause.Value);
			}

			return result;
		}

		private Expression GetEnumeratorExpression(DbExpression predicate, DbModificationCommandTree commandTree, out IReflectionTable table)
		{
			DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor();
			visitor.SetParameters(commandTree.Parameters.ToArray());
			visitor.TableProvider = new DatabaseWrapper(this.DatabaseCache);

			// Get the source expression
			ConstantExpression source = visitor.Visit(commandTree.Target.Expression) as ConstantExpression;

			// This should be a constant expression
			if (source == null)
			{
				throw new InvalidOperationException();
			}

			table = source.Value as IReflectionTable;

			// Get the the type of the elements of the table
			Type elementType = TypeHelper.GetElementType(source.Type);

			// Create context
			ParameterExpression context = Expression.Parameter(elementType, "context");
			using (visitor.CreateVariable(context, commandTree.Target.VariableName))
			{
				// Create the predicate expression
				LambdaExpression predicateExpression =
					Expression.Lambda(
						visitor.Visit(predicate),
						context);

				// Create Where expression
				LinqMethodExpressionBuilder queryMethodBuilder = new LinqMethodExpressionBuilder();

				Expression result = queryMethodBuilder.Where(source, predicateExpression);

				ParameterExpression[] parameterExpressions = visitor.GetParameterExpressions();

				if (parameterExpressions.Length > 0)
				{
					result = Expression.Lambda(result, parameterExpressions);
				}

				return result;
			}
		}

		private IQueryable CreateQuery(Expression expression)
		{
			return DatabaseReflectionHelper.CreateTableQuery(expression, this.DatabaseCache);
		}

		private Func<Dictionary<string, object>, IEnumerable> CreateStoredProcedure(Expression exp)
		{
			var methodCall = exp as MethodCallExpression;

			if (IsQueryable(methodCall) == false)
			{
				var baseCall = methodCall.Arguments[0];

				if (IsQueryable(baseCall) == false)
				{
					throw new InvalidOperationException("Aggregation has to be called on a collection.");
				}

				throw new InvalidOperationException();
			}
			return this.DatabaseCache.CreateStoredProcedure(exp);
		}

		private static bool IsQueryable(Expression methodCall)
		{
			bool isQueryable = false;

			if (methodCall.Type == typeof(IQueryable))
			{
				isQueryable = true;
			}

			if (methodCall.Type.IsGenericType && methodCall.Type.GetGenericTypeDefinition() == typeof(IQueryable<>))
			{
				isQueryable = true;
			}

			return isQueryable;
		}

		#endregion
	}
}
