namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IResultSet
    {
        IEnumerable<IResultSetElement> Elements { get; }
    }
}
