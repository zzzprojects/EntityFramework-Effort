// --------------------------------------------------------------------------------------------
// <copyright file="CachingDataLoader.cs" company="Effort Team">
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
    using System;
    using System.Data.Common;

    /// <summary>
    /// Represents a data loader that serves as a caching layer above another data loader.
    /// </summary>
    public class CachingDataLoader : IDataLoader
    {
        private const string WrappedType = "Type";
        private const string WrappedArgument = "Argument";

        private IDataLoader wrappedDataLoader;

        public CachingDataLoader()
        {
        }

        public CachingDataLoader(IDataLoader wrappedDataLoader)
        {
            if (wrappedDataLoader == null)
            {
                throw new ArgumentNullException("wrappedDataLoader");
            }

            this.wrappedDataLoader = wrappedDataLoader;
        }

        public IDataLoader WrappedDataLoader
        {
            get
            {
                return this.wrappedDataLoader;
            }
        }

        string IDataLoader.Argument
        {
            get
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

                if (this.wrappedDataLoader != null)
                {
                    builder[WrappedType] = 
                        this.wrappedDataLoader.GetType().AssemblyQualifiedName;
                    builder[WrappedArgument] = 
                        this.wrappedDataLoader.Argument;
                }

                return builder.ToString();
            }

            set
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = value;

                string typeName = builder[WrappedType] as string;

                this.wrappedDataLoader = 
                    Activator.CreateInstance(Type.GetType(typeName)) as IDataLoader;

                this.wrappedDataLoader.Argument = 
                    builder[WrappedArgument] as string;
            }
        }

        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            return new CachingTableDataLoaderFactory(this.wrappedDataLoader);
        }
    }
}
