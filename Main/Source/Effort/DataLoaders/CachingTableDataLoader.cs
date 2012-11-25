// --------------------------------------------------------------------------------------------
// <copyright file="CachingTableDataLoader.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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

namespace Effort.DataLoaders
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Represents a table data loader that returns cached data that was retrieved from
    ///     another table data loader.
    /// </summary>
    public class CachingTableDataLoader : ITableDataLoader
    {
        /// <summary>
        ///     The cached data.
        /// </summary>
        private object[][] data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTableDataLoader" /> class.
        /// </summary>
        /// <param name="tableDataLoader">
        ///     The table data loader that is used to retrieve the data.
        /// </param>
        public CachingTableDataLoader(ITableDataLoader tableDataLoader)
        {
            IEnumerable<object[]> data;

            if (tableDataLoader != null)
            {
                data = tableDataLoader.GetData();
            }
            else
            {
                data = Enumerable.Empty<object[]>();
            }

            this.data = data.ToArray();
        }

        /// <summary>
        ///     Creates initial data for the table.
        /// </summary>
        /// <returns>
        ///     The data created for the table.
        /// </returns>
        public IEnumerable<object[]> GetData()
        {
            return this.data;
        }
    }
}
