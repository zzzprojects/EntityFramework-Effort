namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IResultSetElement
    {
        object GetValue(string name);

        bool HasValue(string name);

        string[] FieldNames { get; }
    }
}
