
namespace Effort.Test.Environment.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Objects;
    using Effort.Test.Environment.ResultSets;

    internal interface IQueryTester<TObjectContext> 
        where TObjectContext : ObjectContext
    {
        IResultSet CreateExpectedResult<TResult>(Func<TObjectContext, TResult> queryFactory);

        ICorrectness TestQuery<TResult>(Func<TObjectContext, TResult> queryFactory, string expectedResultJson, bool strictOrder = false);
    }
}
