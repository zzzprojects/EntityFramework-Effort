using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;
using EFProviderWrapperToolkit;

namespace Effort.Test.Utils
{
    internal class QueryTestRuntime<TObjectContext> where TObjectContext : ObjectContext
    {
        private static object lockObject = new object();

        static QueryTestRuntime()
        {
            DatabaseAcceleratorProviderConfiguration.RegisterProvider();
        }

        private string connectionString;

        public QueryTestRuntime(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool Execute<TResult>(Func<TObjectContext, TResult> queryFactory)
        {
            EntityConnection databaseEntityConnection = new EntityConnection(this.connectionString);

            TResult databaseResult = this.ExecuteSingleResultQuery(queryFactory, databaseEntityConnection);
            TResult mmdbResult = this.ExecuteSingleResultQuery(queryFactory, this.CreateWrappedConnection());

            var comparer = EqualityComparers.Create(typeof(TResult));

            return comparer.Equals(databaseResult, mmdbResult);
        }

        public bool Execute<TResult>(Func<TObjectContext, IQueryable<TResult>> queryFactory)
        {
            return this.Execute(queryFactory, false);
        }

        public bool Execute<TResult>(Func<TObjectContext, IQueryable<TResult>> queryFactory, bool strictOrder)
        {
            if (queryFactory == null)
            {
                throw new ArgumentNullException("query");
            }

            EntityConnection databaseEntityConnection = new EntityConnection(this.connectionString);

            List<TResult> databaseResult = this.ExecuteQuery(queryFactory, databaseEntityConnection);
            List<TResult> mmdbResult = this.ExecuteQuery(queryFactory, this.CreateWrappedConnection());

            if (databaseResult.Count != mmdbResult.Count)
            {
                return false;
            }

            CollectionComparer<TResult> comparer = new CollectionComparer<TResult>(strictOrder);

            return comparer.Equals(databaseResult, mmdbResult);
        }

        private EntityConnection CreateWrappedConnection()
        {
            return EntityConnectionWrapperUtils
                .CreateEntityConnectionWithWrappers(this.connectionString, DatabaseAcceleratorProviderConfiguration.ProviderInvariantName);
        }

        private List<TResult> ExecuteQuery<TResult>(Func<TObjectContext, IQueryable<TResult>> queryFactory, EntityConnection connection)
        {
            using (TObjectContext objectContext = this.CreateObjectContext(connection))
            {
                return queryFactory.Invoke(objectContext).ToList();
            }
        }

        private TResult ExecuteSingleResultQuery<TResult>(Func<TObjectContext, TResult> queryFactory, EntityConnection connection)
        {
            using (TObjectContext objectContext = this.CreateObjectContext(connection))
            {
                return queryFactory.Invoke(objectContext);
            }
        }


        private TObjectContext CreateObjectContext(EntityConnection connection)
        {
            TObjectContext objectContext = Activator.CreateInstance(typeof(TObjectContext), connection) as TObjectContext;
            objectContext.ContextOptions.LazyLoadingEnabled = false;
            objectContext.ContextOptions.ProxyCreationEnabled = false;

            return objectContext;
        }
    }
}
