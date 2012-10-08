namespace Effort.Test.Environment.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Effort.Test.Environment.ResultSets;

    internal interface ICorrectness
    {
        void Assert();

        IResultSet Expected { get; }

        IResultSet Actual { get; }
    }
}
