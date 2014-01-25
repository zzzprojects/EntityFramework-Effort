// --------------------------------------------------------------------------------------------
// <copyright file="CachingDataLoader.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
    ///     Represents a data loader that serves as a caching layer above another data loader.
    /// </summary>
    public class CachingDataLoader : IDataLoader
    {
        /// <summary>
        ///     The attribute name of the type of the wrapped data loader in the argument.
        /// </summary>
        private const string WrappedType = "Type";


        /// <summary>
        ///     The attribute name of the argument of the wrapped data loader in the argument.
        /// </summary>
        private const string WrappedArgument = "Argument";

        /// <summary>
        ///     The wrapped data loader.
        /// </summary>
        private IDataLoader wrappedDataLoader;

        /// <summary>
        ///     Indicates if the wrapped data loader should be used only once at the same time.
        /// </summary>
        private bool locking;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingDataLoader" /> class.
        /// </summary>
        public CachingDataLoader()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingDataLoader" /> class.
        /// </summary>
        /// <param name="wrappedDataLoader"> 
        ///     The wrapped data loader. 
        /// </param>
        public CachingDataLoader(IDataLoader wrappedDataLoader)
        {
            if (wrappedDataLoader == null)
            {
                throw new ArgumentNullException("wrappedDataLoader");
            }

            this.wrappedDataLoader = wrappedDataLoader;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingDataLoader" /> class.
        ///     Enabling the <paramref name="locking"/> flag makes the caching data loader 
        ///     instances to work in a cooperative way. They ensure that only one of wrapped
        ///     data loaders initialized with the same configuration is utilized at the same
        ///     time. 
        /// </summary>
        /// <param name="wrappedDataLoader"> 
        ///     The wrapped data loader. 
        /// </param>
        /// <param name="locking">
        ///     Indicates if the wrapped data loader should be used only once at the same time.
        /// </param>
        public CachingDataLoader(IDataLoader wrappedDataLoader, bool locking)
        {
            if (wrappedDataLoader == null)
            {
                throw new ArgumentNullException("wrappedDataLoader");
            }

            this.wrappedDataLoader = wrappedDataLoader;
            this.locking = locking;
        }

        /// <summary>
        ///     Gets the wrapped data loader.
        /// </summary>
        /// <value>
        ///     The wrapped data loader.
        /// </value>
        public IDataLoader WrappedDataLoader
        {
            get
            {
                return this.wrappedDataLoader;
            }
        }

        /// <summary>
        ///     Gets or sets the argument that describes the complete state of the data loader.
        /// </summary>
        /// <value>
        ///     The argument.
        /// </value>
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

        /// <summary>
        ///     Creates a table data loader factory.
        /// </summary>
        /// <returns>
        ///     A table data loader factory.
        /// </returns>
        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            return new CachingTableDataLoaderFactory(this.wrappedDataLoader, this.locking);
        }
    }
}
