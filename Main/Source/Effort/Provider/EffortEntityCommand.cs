// --------------------------------------------------------------------------------------------
// <copyright file="EffortEntityCommand.cs" company="Effort Team">
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
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using Effort.Internal.CommandActions;

    /// <summary>
    ///     Represent an Effort command that realizes Entity Framework command tree 
    ///     representations.
    /// </summary>
    public sealed class EffortEntityCommand : EffortCommandBase, ICloneable
    {
        private ICommandAction commandAction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortEntityCommand" /> class 
        ///     based on  a provided command tree.
        /// </summary>
        /// <param name="commandtree">
        ///     The command tree that describes the operation.
        /// </param>
        public EffortEntityCommand(DbCommandTree commandtree)
        {
            this.commandAction = CommandActionFactory.Create(commandtree);

            foreach (KeyValuePair<string, TypeUsage> param in commandtree.Parameters)
            {
                this.AddParameter(param.Key);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortEntityCommand" /> class 
        ///     based on a prototype instance.
        /// </summary>
        /// <param name="prototype">
        ///     The prototype <see cref="EffortEntityCommand" /> object.
        /// </param>
        private EffortEntityCommand(EffortEntityCommand prototype)
        {
            this.commandAction = prototype.commandAction;

            foreach (EffortParameter parameter in prototype.Parameters)
            {
                this.AddParameter(parameter.ParameterName);
            }
        }

        /// <summary>
        ///     Executes the query.
        /// </summary>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public override int ExecuteNonQuery()
        {
            ActionContext context = this.CreateActionContext();

            return this.commandAction.ExecuteNonQuery(context);
        }

        /// <summary>
        ///     Executes the query and returns the first column of the first row in the result
        ///     set returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>
        ///     The first column of the first row in the result set.
        /// </returns>
        public override object ExecuteScalar()
        {
            ActionContext context = this.CreateActionContext();

            return this.commandAction.ExecuteScalar(context);
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new EffortEntityCommand(this);
        }

        /// <summary>
        ///     Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">
        ///     An instance of <see cref="T:System.Data.CommandBehavior" />.
        /// </param>
        /// <returns>
        ///     A <see cref="EffortDataReader" />.
        /// </returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ActionContext context = this.CreateActionContext();

            return this.commandAction.ExecuteDataReader(context);
        }

        private ActionContext CreateActionContext()
        {
            ActionContext context = new ActionContext(this.EffortConnection.DbContainer);

            // Store parameters in the context
            foreach (DbParameter parameter in this.Parameters)
            {
                CommandActionParameter commandActionParameter =
                    new CommandActionParameter(parameter.ParameterName, parameter.Value);

                context.Parameters.Add(commandActionParameter);
            }

            if (this.EffortTransaction != null)
            {
                context.Transaction = this.EffortTransaction.InternalTransaction;
            }

            return context;
        }
    }
}
