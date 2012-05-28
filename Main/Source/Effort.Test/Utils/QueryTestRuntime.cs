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
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Linq;

namespace Effort.Test.Utils
{
    internal class QueryTestRuntime<TObjectContext> where TObjectContext : ObjectContext
    {
        private static object lockObject = new object();

        static QueryTestRuntime()
        {
            //DatabaseAcceleratorProviderConfiguration.RegisterProvider();
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
            TResult mmdbResult = this.ExecuteSingleResultQuery(queryFactory, this.CreateEmulatorConnection());

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
            List<TResult> mmdbResult = this.ExecuteQuery(queryFactory, this.CreateEmulatorConnection());

            if (databaseResult.Count != mmdbResult.Count)
            {
                return false;
            }

            CollectionComparer<TResult> comparer = new CollectionComparer<TResult>(strictOrder);

            return comparer.Equals(databaseResult, mmdbResult);
        }

        private EntityConnection CreateEmulatorConnection()
        {
            return null;
            //return EntityConnectionWrapperUtils
            //    .CreateEntityConnectionWithWrappers(this.connectionString, "");//DatabaseAcceleratorProviderConfiguration.ProviderInvariantName);
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
