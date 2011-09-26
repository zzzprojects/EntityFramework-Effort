using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Data.Metadata.Edm;

using EFProviderWrapperToolkit;
using System.Data.Common.CommandTrees;
using System.Linq.Expressions;

using MMDB.Linq.Modifiers;
using MMDB.Table;
using MMDB.EntityFrameworkProvider.DatabaseManagement;
using MMDB.EntityFrameworkProvider.Helpers;
using MMDB.EntityFrameworkProvider.DbCommandTreeTransform;
using MMDB.EntityFrameworkProvider.DbCommandTreeTransform.PostProcessing;
using MMDB.Logging;
using MMDB.Linq;
using MMDB.Linq.Visitors;

namespace MMDB.EntityFrameworkProvider.Components
{
    public class MMDBWrapperCommand : DbCommandWrapper
    {
        private bool isDbTransactionUsed = false;

        public MMDBWrapperCommand( DbCommand wrappedCommand, DbCommandDefinitionWrapper commandDefinition )
            : base( wrappedCommand, commandDefinition )
        {

        }

        public Database DatabaseCache
        {
            get
            {
                return this.WrapperConnection.DatabaseCache;
            }
        }

        public MMDBWrapperConnection WrapperConnection
        {
            get
            {
                MMDBWrapperConnection connection = base.Connection as MMDBWrapperConnection;

                if( connection == null )
                {
                    throw new InvalidOperationException();
                }

                return connection;
            }
        }

        #region Execute methods

        public override int ExecuteNonQuery()
        {
            if( this.DesignMode )
            {
                return base.ExecuteNonQuery();
            }

            if( this.Definition.CommandTree is DbUpdateCommandTree )
            {
                return this.PerformUpdate();
            }
            else if( this.Definition.CommandTree is DbDeleteCommandTree )
            {
                return this.PerformDelete();
            }
            else if( this.Definition.CommandTree is DbInsertCommandTree )
            {
                return this.PerformInsert();
            }

            throw new NotSupportedException();
        }

        protected override DbDataReader ExecuteDbDataReader( CommandBehavior behavior )
        {
            if( this.DesignMode )
            {
                return base.ExecuteDbDataReader( behavior );
            }

            if( this.Definition.CommandTree is DbInsertCommandTree )
            {
                return PerformInsert( behavior );
            }
            else if( base.Definition.CommandTree is DbQueryCommandTree )
            {
                return PerformQuery( behavior );
            }

            throw new NotSupportedException();
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return base.DbTransaction;
            }
            set
            {
                // DbTransaction is only used virtually, the transactions are always distributed (db + mmdb),
                // so handled by an ambient transaction (System.Transactions.Transaction)
                base.DbTransaction = null;

                // Note, that the virtual DbTransaction is used
                this.isDbTransactionUsed = value != null;
            }
        }


        #endregion

        #region Query

        private DbDataReader PerformQuery( CommandBehavior behavior )
        {
            Console.WriteLine( this.CommandText );
            DbQueryCommandTree commandTree = base.Definition.CommandTree as DbQueryCommandTree;

            Stopwatch stopWatch = Stopwatch.StartNew();

            #region Skip Cache

            if( this.WrapperConnection.DefaultCacheMode == false || this.WrapperConnection.NonCached.Any() )
            {
                TableScanVisitor tableScanVisitor = new TableScanVisitor();
                tableScanVisitor.Visit( commandTree.Query );

                bool skipCache = false;
                if( this.WrapperConnection.DefaultCacheMode && tableScanVisitor.Tables.Any( t => this.WrapperConnection.NonCached.Contains( t ) ) )
                    skipCache = true;
                if( this.WrapperConnection.DefaultCacheMode == false && tableScanVisitor.Tables.Any( t => this.WrapperConnection.Cached.Contains( t ) ) == false )
                    skipCache = true;

                if( skipCache )
                {
                    // Not using MMDB Cache
                    Console.WriteLine( "Not using MMDB Cache for the current query." );
                    this.EnsureOpenConnection();
                    return base.WrappedCommand.ExecuteReader( behavior );
                }
            }
            #endregion

            //Setup expression tranformer
            DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor();
            visitor.SetParameters( commandTree.Parameters.ToArray() );
            visitor.TableProvider = new DatabaseWrapper( this.DatabaseCache );

            Expression linqExpression = null;
            Func<Dictionary<string, object>, IEnumerable> storedProc = null;

            bool hasParameters = this.Parameters.Count > 0;
            bool cached = false;

            #region Transform Cache
            if( hasParameters == false &&
                this.DatabaseCache.TransformCache.TryGetValue( this.CommandText, out linqExpression ) )
            {
                cached = true;
            }
            if( hasParameters && this.DatabaseCache.TransformCacheProcedures.TryGetValue( this.CommandText, out storedProc ) )
            {
                cached = true;
            }
            if( cached && this.DatabaseCache.LoggingPort != null )
            {
                this.DatabaseCache.LoggingPort.Send( new StandardMMDBMessage( "TransformCache used." ) );
            }

            #endregion

            // Check if this expression tree has not been cached yet
            if( cached == false )
            {
                // Convert DbCommandTree to linq Expression Tree
                linqExpression = visitor.Visit( commandTree.Query );

                #region Postprocessing

                //Convert ' Nullable<?> Enumerable.Sum ' to ' Nullable<?> EnumerableNullSum.Sum '
                SumTransformerVisitor sumTransformer = new SumTransformerVisitor();
                linqExpression = sumTransformer.Visit( linqExpression );

                //Clean ' new SingleResult<>(x).FirstOrDefault() '
                ExcrescentSingleResultCleanserVisitor cleanser1 = new ExcrescentSingleResultCleanserVisitor();
                linqExpression = cleanser1.Visit( linqExpression );

                //Clean ' new AT(x).YProp '    (x is the init value of YProp)
                ExcrescentInitializationCleanserVisitor cleanser2 = new ExcrescentInitializationCleanserVisitor();
                linqExpression = cleanser2.Visit( linqExpression );

                #endregion

                // Place the transformed expression into the cache
                if( hasParameters )
                {
                    this.DatabaseCache.TransformCacheProcedures[this.CommandText] = storedProc = this.CreateStoredProcedure( linqExpression );
                }
                else
                {
                    this.DatabaseCache.TransformCache[this.CommandText] = linqExpression;
                }
            }

            if( this.DatabaseCache.LoggingPort != null )
            {
                this.DatabaseCache.LoggingPort.Send(
                    new Logging.StandardMMDBMessage(
                        "DbCommandTree converted in {0:0.00} ms",
                        stopWatch.Elapsed.TotalMilliseconds ) );

            }


            Console.WriteLine();
            Console.WriteLine( new ExpressionFormatter().Format( linqExpression, 299 ) );
            Console.WriteLine();

            if( hasParameters )
            {
                Dictionary<string, object> parameters = this
                     .Parameters
                     .Cast<DbParameter>()
                     .ToDictionary( p => p.ParameterName, p => p.Value as object );

                return new MMDBDataReader( storedProc( parameters ) );
            }
            else
            {
                IQueryable result = this.CreateQuery( linqExpression );

                return new MMDBDataReader( result );
            }
        }

        #endregion

        #region Update

        private int PerformUpdate()
        {
            this.EnsureOpenConnection();

            //Execute update on the database
            int? result = null;

            if( this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator )
            {
                result = base.WrappedCommand.ExecuteNonQuery();
            }

            DbUpdateCommandTree commandTree = base.Definition.CommandTree as DbUpdateCommandTree;

            IReflectionTable table = null;

            Expression linqExpression = this.GetEnumeratorExpression( commandTree.Predicate, commandTree, out table );
            IEnumerable entitiesToUpdate = this.CreateQuery( linqExpression );

            Type type = TypeHelper.GetEncapsulatedType( table.GetType() );

            //Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions( commandTree.SetClauses, table );

            //Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

            //Setup context for the predicate
            ParameterExpression context = Expression.Parameter( type, "context" );
            using( transform.CreateContext( context, commandTree.Target.VariableName ) )
            {
                //Initialize member bindings
                foreach( PropertyInfo property in type.GetProperties() )
                {
                    //Check if member has set clause
                    if( setClauses.ContainsKey( property.Name ) )
                    {
                        //Set the clause
                        memberBindings.Add(
                            Expression.Bind(
                                property,
                                transform.Visit( setClauses[property.Name] ) ) );
                    }
                    else
                    {
                        //Copy member
                        memberBindings.Add(
                            Expression.Bind(
                                property,
                                Expression.Property( context, property ) ) );
                    }
                }
            }

            Delegate updater =
                Expression.Lambda(
                    Expression.MemberInit( Expression.New( type ), memberBindings ),
                    context )
                .Compile();


            int counter = 0;

            //Execute update on the table
            foreach( object entity in entitiesToUpdate )
            {
                object updatedEntity = updater.DynamicInvoke( entity );

                table.Update( entity, updatedEntity );
                counter++;
            }

            // Compare the result count in accelerator mode 
            if( result.HasValue && counter != result.Value )
            {
                throw new InvalidOperationException();
            }

            return counter;
        }

        #endregion

        #region Delete

        private int PerformDelete()
        {
            this.EnsureOpenConnection();

            int? result = null;

            //Execute delete on the database
            if( this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator )
            {
                result = base.ExecuteNonQuery();
            }

            DbDeleteCommandTree commandTree = base.Definition.CommandTree as DbDeleteCommandTree;

            IReflectionTable table = null;

            Expression linqExpression = this.GetEnumeratorExpression( commandTree.Predicate, commandTree, out table );
            IEnumerable entitiesToDeleteQuery = this.CreateQuery( linqExpression );

            List<object> entitiesToDeleteList = entitiesToDeleteQuery.Cast<object>().ToList();

            // Compare the delete in accelerator mode
            if( result.HasValue && result != entitiesToDeleteList.Count )
            {
                throw new InvalidOperationException();
            }

            int count = 0;

            //Execute delete on the table
            foreach( object entity in entitiesToDeleteList )
            {
                table.Delete( entity );
                count++;
            }

            // Compare the result count in accelerator mode
            if( result.HasValue && result.Value != count )
            {
                throw new InvalidOperationException();
            }

            return count;
        }

        #endregion

        #region Insert

        private int PerformInsert()
        {
            this.EnsureOpenConnection();

            if( WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator )
            {
                base.WrappedCommand.ExecuteNonQuery();
            }

            DbInsertCommandTree commandTree = base.Definition.CommandTree as DbInsertCommandTree;
            //Get the source table
            IReflectionTable table = this.GetTable( commandTree );

            //Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions( commandTree.SetClauses, table );

            //Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

            //Initialize member bindings
            foreach( PropertyInfo property in table.EntityType.GetProperties() )
            {
                //Check if member has set clause
                if( setClauses.ContainsKey( property.Name ) )
                {
                    //Set the clause
                    memberBindings.Add(
                        Expression.Bind(
                            property,
                            transform.Visit( setClauses[property.Name] ) ) );
                }
            }

            this.CreateAndInsertEntity( table, memberBindings );

            return 1;
        }

        private DbDataReader PerformInsert( CommandBehavior behavior )
        {
            if( this.WrapperConnection.ProviderMode == ProviderModes.DatabaseEmulator )
            {
                // Identity is not supported yet
                throw new NotSupportedException();
            }

            this.EnsureOpenConnection();

            DbDataReader reader = base.WrappedCommand.ExecuteReader( behavior );
            object returnValue = null;

            using( reader )
            {
                if( !reader.Read() )
                {
                    throw new InvalidOperationException();
                }

                returnValue = reader.GetValue( 0 );
            }

            DbInsertCommandTree commandTree = base.Definition.CommandTree as DbInsertCommandTree;
            //Get the source table
            IReflectionTable table = this.GetTable( commandTree );

            //Determine the returning field
            string keyPropertyName = null;

            if( commandTree.Returning != null )
            {
                DbNewInstanceExpression returnExpression = commandTree.Returning as DbNewInstanceExpression;

                if( returnExpression == null )
                {
                    throw new NotSupportedException( "The type of the Returning properties is not DbNewInstanceExpression" );
                }

                if( returnExpression.Arguments.Count > 1 )
                {
                    throw new NotSupportedException( "The DbNewInstanceExpression contains more than one argument" );
                }

                DbPropertyExpression propertyExpression = returnExpression.Arguments[0] as DbPropertyExpression;

                if( propertyExpression == null )
                {
                    throw new NotSupportedException( "The type of the argument of the DbNewInstanceExpression is not PropertyExpression" );
                }

                keyPropertyName = propertyExpression.Property.Name;
            }

            //Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = this.GetSetClauseExpressions( commandTree.SetClauses, table );

            //Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor();

            //Initialize member bindings
            foreach( PropertyInfo property in table.EntityType.GetProperties() )
            {
                //Check if member has set clause
                if( setClauses.ContainsKey( property.Name ) )
                {
                    Expression value = transform.Visit( setClauses[property.Name] );

                    // zsvarnai: Megint nullable gondok vannak es nem lehet updatelni pl boolt
                    if( property.PropertyType.IsGenericType == false || property.PropertyType.GetGenericTypeDefinition() != typeof( Nullable<> ) )
                    {
                        value = Expression.Convert( value, property.PropertyType );
                    }

                    //Set the clause
                    memberBindings.Add(
                        Expression.Bind(
                            property,
                            value ) );
                }
                else if( property.Name == keyPropertyName )
                {
                    //Set the clause
                    memberBindings.Add(
                        Expression.Bind(
                            property,
                            Expression.Constant( returnValue ) ) );
                }
            }

            CreateAndInsertEntity( table, memberBindings );

            return new MMDBDataReader( new[] { returnValue }, true ) { PrimitiveName = keyPropertyName };
        }

        private IReflectionTable GetTable( DbInsertCommandTree commandTree )
        {
            DbScanExpression source = commandTree.Target.Expression as DbScanExpression;

            if( source == null )
            {
                throw new InvalidOperationException( "The type of the Target property is not DbScanExpression" );
            }

            return this.DatabaseCache.GetTable( source.Target.Name );
        }

        private void CreateAndInsertEntity( IReflectionTable table, IList<MemberBinding> memberBindings )
        {
            Delegate factory =
               Expression.Lambda(
                   Expression.MemberInit(
                       Expression.New( table.EntityType ),
                       memberBindings ) )
               .Compile();

            object newEntity = factory.DynamicInvoke();

            table.Insert( newEntity );
        }

        #endregion

        #region Helper methods

        private void EnsureOpenConnection()
        {
            // Open the wrapped connection, if it has not been yet, and the mode is accelerator
            if( this.WrapperConnection.ProviderMode == ProviderModes.DatabaseAccelerator &&
                base.WrappedCommand.Connection.State == ConnectionState.Closed )
            {
                base.WrappedCommand.Connection.Open();

                // Check for an ambient transaction
                var transaction = System.Transactions.Transaction.Current;

                // If DbTransaction is used, then the wrapped connection is already enlisted
                // Note: In this case Transaction.Current should be null
                if( !this.isDbTransactionUsed && transaction != null )
                {
                    this.WrappedCommand.Connection.EnlistTransaction( transaction );
                }
            }
        }


        private IDictionary<string, DbExpression> GetSetClauseExpressions( IList<DbModificationClause> clauses, object table )
        {
            Type type = TypeHelper.GetEncapsulatedType( table.GetType() );

            IDictionary<string, DbExpression> result = new Dictionary<string, DbExpression>();

            foreach( DbSetClause setClause in clauses.Cast<DbSetClause>() )
            {
                DbPropertyExpression property = setClause.Property as DbPropertyExpression;

                if( property == null )
                {
                    throw new NotSupportedException( setClause.Property.ExpressionKind.ToString() + " is not supported" );
                }

                result.Add( property.Property.Name, setClause.Value );
            }

            return result;
        }

        private Expression GetEnumeratorExpression( DbExpression predicate, DbModificationCommandTree commandTree, out IReflectionTable table )
        {
            DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor();
            visitor.SetParameters( commandTree.Parameters.ToArray() );
            visitor.TableProvider = new DatabaseWrapper( this.DatabaseCache );

            // Get the source expression
            ConstantExpression source = visitor.Visit( commandTree.Target.Expression ) as ConstantExpression;

            // This should be a constant expression
            if( source == null )
            {
                throw new InvalidOperationException();
            }

            table = source.Value as IReflectionTable;

            // Get the the type of the elements of the table
            Type elementType = TypeHelper.GetEncapsulatedType( source.Type );

            // Create context
            ParameterExpression context = Expression.Parameter( elementType, "context" );
            using( visitor.CreateContext( context, commandTree.Target.VariableName ) )
            {
                // Create the predicate expression
                LambdaExpression predicateExpression =
                    Expression.Lambda(
                        visitor.Visit( predicate ),
                        context );

                // Create Where expression
                QueryMethodExpressionBuilder queryMethodBuilder = new QueryMethodExpressionBuilder();

                Expression result = queryMethodBuilder.Where( source, predicateExpression );

                ParameterExpression[] parameterExpressions = visitor.GetParameterExpressions();

                if( parameterExpressions.Length > 0 )
                {
                    result = Expression.Lambda( result, parameterExpressions );
                }

                return result;
            }
        }

        private IQueryable CreateQuery( Expression expression )
        {
            TableQueryProvider<object> provider = new TableQueryProvider<object>( this.DatabaseCache );
            TableQuery<object> query = new TableQuery<object>( provider, expression );

            return query;
        }

        private Func<Dictionary<string, object>, IEnumerable> CreateStoredProcedure( Expression exp )
        {
            var methodCall = exp as MethodCallExpression;

            if( IsQueryable( methodCall ) == false )
            {
                var baseCall = methodCall.Arguments[0];
                if( IsQueryable( baseCall ) == false )
                    throw new InvalidOperationException( "Aggregation has to be called on a collection." );

                throw new InvalidOperationException();
            }
            return this.DatabaseCache.CreateStoredProcedure( exp );
        }

        private static bool IsQueryable( Expression methodCall )
        {
            bool isQueryable = false;
            if( methodCall.Type == typeof( IQueryable ) )
                isQueryable = true;
            if( methodCall.Type.IsGenericType && methodCall.Type.GetGenericTypeDefinition() == typeof( IQueryable<> ) )
                isQueryable = true;
            return isQueryable;
        }

        #endregion
    }
}
