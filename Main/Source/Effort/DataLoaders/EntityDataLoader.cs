// --------------------------------------------------------------------------------------------
// <copyright file="EntityDataLoader.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
    using System.Data.EntityClient;

    /// <summary>
    ///     Represents a data loader that loads data from a database that has an Entity 
    ///     Framework provider registered.
    /// </summary>
    public class EntityDataLoader : IDataLoader
    {
        private string entityConnectionString;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityDataLoader" /> class.
        /// </summary>
        public EntityDataLoader()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityDataLoader" /> class.
        /// </summary>
        /// <param name="entityConnectionString"> The entity connection string. </param>
        public EntityDataLoader(string entityConnectionString)
        {
            this.entityConnectionString = entityConnectionString;
        }

        /// <summary>
        ///     Gets or sets the argument that contains the entity connection string that 
        ///     references to the source database.
        /// </summary>
        /// <value>
        ///     The argument.
        /// </value>
        string IDataLoader.Argument
        {
            get
            {
                return this.entityConnectionString;
            }

            set
            {
                this.entityConnectionString = value;
            }
        }

        /// <summary>
        ///     Creates a <see cref="EntityTableDataLoaderFactory" /> instance.
        /// </summary>
        /// <returns>
        ///     The <see cref="EntityTableDataLoaderFactory" /> instance.
        /// </returns>
        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            return new EntityTableDataLoaderFactory(
                () => new EntityConnection(this.entityConnectionString));
        }
    }
}
