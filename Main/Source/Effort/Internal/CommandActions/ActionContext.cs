// --------------------------------------------------------------------------------------------
// <copyright file="ActionContext.cs" company="Effort Team">
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

namespace Effort.Internal.CommandActions
{
    using System.Collections.Generic;
    using System.Data;
    using Effort.Internal.DbManagement;
    using NMemory.Transactions;

    /// <summary>
    /// Containes information about a command execution environment.
    /// </summary>
    internal sealed class ActionContext
    {
        /// <summary>
        /// The database container that the command is executed on.
        /// </summary>
        private DbContainer container;

        /// <summary>
        /// The parameters of the command action.
        /// </summary>
        private IList<CommandActionParameter> parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionContext" /> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public ActionContext(DbContainer container)
        {
            this.container = container;
            this.parameters = new List<CommandActionParameter>();
        }

        /// <summary>
        /// Gets the database container that the command should be executed on.
        /// </summary>
        /// <value>
        /// The db container.
        /// </value>
        public DbContainer DbContainer
        {
            get 
            { 
                return this.container; 
            }
        }

        /// <summary>
        /// Gets the collection of the parameters of the command action.
        /// </summary>
        /// <value>
        /// The collection of the command action parameters.
        /// </value>
        public IList<CommandActionParameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        /// Gets or sets the transaction that the command action is executed within.
        /// </summary>
        /// <value>
        /// The transaction.
        /// </value>
        public Transaction Transaction 
        { 
            get; 
            
            set;
        }
    }
}
