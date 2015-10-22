// --------------------------------------------------------------------------------------------
// <copyright file="QueryTester`1.cs" company="Effort Team">
//     Copyright (C) Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.Queries
{
    using System;
    using System.Collections;
#if !EFOLD
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.EntityClient;
    using System.Data.Objects;
#endif
    using Effort.DataLoaders;
    using Effort.Test.Internal.ResultSets;

    internal class QueryTester<TObjectContext> :
        IQueryTester<TObjectContext> 
        where TObjectContext : ObjectContext
    {
        private string connectionString;
        private IDataLoader dataLoader;

        public QueryTester(string connectionString, IDataLoader dataLoader)
        {
            this.connectionString = connectionString;
            this.dataLoader = dataLoader;
        }

        public IResultSet CreateExpectedResult<TResult>(Func<TObjectContext, TResult> queryFactory)
        {
            return
                this.GetResultSet(
                    CreateInspectedStoreConnection,
                    ctx => queryFactory(ctx));
        }

        public ICorrectness TestQuery<TResult>(Func<TObjectContext, TResult> queryFactory, string expectedResultJson, bool strictOrder = false)
        {
            IResultSet expectedResult = null;
            IResultSet actualResult = null;

            // If no expected result was specified, execute on the database
            if (string.IsNullOrEmpty(expectedResultJson))
            {
                expectedResult = CreateExpectedResult(queryFactory);
                actualResult = new JsonResultSet("[]");
            }
            else
            {
                expectedResult = new JsonResultSet(expectedResultJson);
                actualResult = this.GetResultSet(
                    CreateInspectedFakeConnection,
                    ctx => queryFactory(ctx));
            }

            return new Correctness(expectedResult, actualResult, strictOrder);
        }

        private IResultSet GetResultSet<TResult>(
            Func<IResultSetComposer, EntityConnection> entityConnectionFactory,
            Func<TObjectContext, TResult> queryAction)
        {
            IResultSetComposer composer = new DictionaryResultSetComposer();

            using (EntityConnection connection = entityConnectionFactory.Invoke(composer))
            using (TObjectContext objectContext = CreateObjectContext(connection))
            {
                IEnumerable enumerable = queryAction.Invoke(objectContext) as IEnumerable;

                if (enumerable != null)
                {
                    IEnumerator enumerator = enumerable.GetEnumerator();

                    while (enumerator.MoveNext())
                        ;
                }
            }

            // This step is required to make query result compatible with jsonized expected result expressions
            string json = ResultSetJsonSerializer.Serialize(composer.ResultSet);

            return new JsonResultSet(json);
        }

        private EntityConnection CreateInspectedStoreConnection(IResultSetComposer composer)
        {
            return EntityConnectionHelper.CreateInspectedEntityConnection(this.connectionString, composer);
        }

        private EntityConnection CreateInspectedFakeConnection(IResultSetComposer composer)
        {
            return EntityConnectionHelper.CreateInspectedFakeEntityConnection(this.connectionString, composer, this.dataLoader);
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
