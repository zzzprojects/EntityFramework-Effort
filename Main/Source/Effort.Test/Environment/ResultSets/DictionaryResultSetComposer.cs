// ----------------------------------------------------------------------------------
// <copyright file="DictionaryResultSetComposer.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Test.Environment.ResultSets
{
    using System.Collections.Generic;

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
