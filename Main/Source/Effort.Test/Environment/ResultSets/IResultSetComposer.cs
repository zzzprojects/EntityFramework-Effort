namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal interface IResultSetComposer
    {
        IResultSet ResultSet { get; }

        void SetValue<T>(string name, T value);

        void Commit();
    }
}
