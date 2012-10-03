#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using Effort.Internal.CommandActions;

namespace Effort.Provider
{
    public class EffortEntityCommand : EffortCommandBase, ICloneable
    {
        private DbCommandTree commandTree;

        public EffortEntityCommand(DbCommandTree commandtree)
        {
            this.commandTree = commandtree;

            foreach (KeyValuePair<string, TypeUsage> param in commandtree.Parameters)
            {
                this.Parameters.Insert(0, new EffortParameter() { ParameterName = param.Key });
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ActionContext context = this.CreateActionContext();
            context.CommandBehavior = behavior;

            ICommandAction action = CreateCommandAction();

            return action.ExecuteDataReader(this.commandTree, context);
        }

        public override int ExecuteNonQuery()
        {
            ActionContext context = this.CreateActionContext();

            ICommandAction action = this.CreateCommandAction();

            return action.ExecuteNonQuery(this.commandTree, context);
        }

        public override object ExecuteScalar()
        {
            ActionContext context = this.CreateActionContext();

            ICommandAction action = this.CreateCommandAction();

            return action.ExecuteScalar(this.commandTree, context);
        }

     

        public object Clone()
        {
            return new EffortEntityCommand(this.commandTree);
        }

        private ActionContext CreateActionContext()
        {
            ActionContext context = new ActionContext(this.EffortConnection.DbContainer);

            // Store parameters in the context
            foreach (DbParameter parameter in this.Parameters)
            {
                context.Parameters.Add(new Parameter(parameter.ParameterName, parameter.Value));
            }

            if (this.EffortTransaction != null)
            {
                context.Transaction = this.EffortTransaction.InternalTransaction;
            }

            return context;
        }

        private ICommandAction CreateCommandAction()
        {
            ICommandAction action = null;

            if (this.commandTree is DbQueryCommandTree)
            {
                action = new QueryCommandAction();
            }
            else if (this.commandTree is DbInsertCommandTree)
            {
                action = new InsertCommandAction();
            }
            else if (this.commandTree is DbUpdateCommandTree)
            {
                action = new UpdateCommandAction();
            }
            else if (this.commandTree is DbDeleteCommandTree)
            {
                action = new DeleteCommandAction();
            }

            if (action == null)
            {
                throw new NotSupportedException("Not supported DbCommandTree type");
            }
            return action;
        }
    }
}
