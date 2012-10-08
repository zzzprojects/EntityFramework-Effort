namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    internal class DictionaryResultSet : IResultSet
    {
        IDictionary<string, object>[] elements;

        public DictionaryResultSet(IDictionary<string, object>[] elements)
        {
            this.elements = elements;
        }
        public IEnumerable<IResultSetElement> Elements
        {
            get 
            { 
                return this.elements
                    .Select(x => new DictionaryResultSetElement(x)); 
            }
        }
    }
}
