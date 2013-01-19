// --------------------------------------------------------------------------------------------
// <copyright file="EffortConnectionStringBuilder.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Provider
{
    using System;
    using System.Data.Common;
    using Effort.DataLoaders;

    /// <summary>
    ///     Providers a simple way to manage the contents of connection string used by the
    ///     <see cref="EffortConnection"/> class.
    /// </summary>
    public class EffortConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static readonly string InstanceIdKey = "InstanceId";
        private static readonly string DataLoaderTypeKey = "DataLoaderType";
        private static readonly string DataLoaderArgKey = "DataLoaderArg";

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortConnectionStringBuilder" /> 
        ///     class.
        /// </summary>
        public EffortConnectionStringBuilder()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortConnectionStringBuilder" /> 
        ///     class. The provided connection string provides the data for the internal
        ///     connection information of the instance.
        /// </summary>
        /// <param name="connectionString">
        ///     The basis for the object's internal connection information.
        /// </param>
        public EffortConnectionStringBuilder(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        ///     Gets or sets the string that identifies the database instance.
        /// </summary>
        /// <value>
        ///     The identifier of the database instance.
        /// </value>
        public string InstanceId
        {
            get
            {
                if (!this.ContainsKey(InstanceIdKey))
                {
                    return string.Empty;
                }

                return this[InstanceIdKey] as string;
            }

            set
            {
                this[InstanceIdKey] = value;
            }
        }

        /// <summary>
        ///     Gets or sets the type of the data loader that is used to initialize the state
        ///     of the database instance. It has to implement the 
        ///     <see cref="Effort.DataLoaders.IDataLoader"/> interface.
        /// </summary>
        /// <value>
        ///     The type of the data loader. 
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        ///     Cannot set data loader.
        /// </exception>
        public Type DataLoaderType
        {
            get
            {
                if (!this.ContainsKey(DataLoaderTypeKey))
                {
                    return null;
                }

                string assemblyQualifiedName = this[DataLoaderTypeKey] as string;

                return Type.GetType(assemblyQualifiedName);
            }

            set
            {
                if (value == null)
                {
                    this[DataLoaderTypeKey] = null;
                    return;
                }

                // Check the type validity
                if (typeof(IDataLoader).IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException("Invalid type", "value");
                }

                this[DataLoaderTypeKey] = value.AssemblyQualifiedName;

                if (this.DataLoaderType != value)
                {
                    throw new InvalidOperationException("Cannot set dataloader");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the data loader argument that is used by the data loader to
        ///     initialize the state of the database.
        /// </summary>
        /// <value>
        ///     The data loader argument.
        /// </value>
        public string DataLoaderArgument
        {
            get
            {
                if (!this.ContainsKey(DataLoaderArgKey))
                {
                    return string.Empty;
                }

                return this[DataLoaderArgKey] as string;
            }

            set
            {
                this[DataLoaderArgKey] = value;
            }
        }
    }
}
