namespace Effort.Test.Environment.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Effort.Test.Data.Northwind;

    internal class NorthwindQueryTester : QueryTester<NorthwindObjectContext>
    {
        public NorthwindQueryTester() 
            : base(NorthwindObjectContext.DefaultConnectionString, new NorthwindLocalDataLoader())
        {
                
        }
    }
}
