namespace Effort.Test.Environment.ResultSets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class DictionaryResultSetComposer : IResultSetComposer
    {
        private IResultSet createdResultSet;

        private IDictionary<string, object> current;
        private List<IDictionary<string, object>> resultSet;

        public DictionaryResultSetComposer()
        {
            this.createdResultSet = null;
            this.current = new Dictionary<string, object>();

            this.resultSet = new List<IDictionary<string, object>>();
        }

        public IResultSet ResultSet
        {
            get 
            {
                if (this.createdResultSet == null)
                {
                    this.CreateResultSet();
                }

                return this.createdResultSet;
            }
        }

        public void SetValue<T>(string name, T value)
        {
            this.current[name] = value;
        }

        public void Commit()
        {
            this.resultSet.Add(this.current);

            this.current = new Dictionary<string, object>();
        }

        private void CreateResultSet() 
        {
            this.createdResultSet = new DictionaryResultSet(this.resultSet.ToArray());
        }
    }
}
