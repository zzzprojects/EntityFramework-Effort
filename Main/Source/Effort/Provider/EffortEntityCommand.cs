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
        private ICommandAction commandAction;

        public EffortEntityCommand(DbCommandTree commandtree)
        {
            this.commandAction = CommandActionFactory.Create(commandtree);

            foreach (KeyValuePair<string, TypeUsage> param in commandtree.Parameters)
            {
                this.Parameters.Insert(0, new EffortParameter() { ParameterName = param.Key });
            }
        }

        private EffortEntityCommand(EffortEntityCommand prototype)
        {
            this.commandAction = prototype.commandAction;

            foreach (EffortParameter parameter in prototype.Parameters)
            {
                this.Parameters.Insert(0, new EffortParameter() { ParameterName = parameter.ParameterName });
            }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            ActionContext context = this.CreateActionContext();
            context.CommandBehavior = behavior;

            return this.commandAction.ExecuteDataReader(context);
        }

        public override int ExecuteNonQuery()
        {
            ActionContext context = this.CreateActionContext();

            return this.commandAction.ExecuteNonQuery(context);
        }

        public override object ExecuteScalar()
        {
            ActionContext context = this.CreateActionContext();

            return this.commandAction.ExecuteScalar(context);
        }

 
        public object Clone()
        {
            return new EffortEntityCommand(this);
        }

        private ActionContext CreateActionContext()
        {
            ActionContext context = new ActionContext(this.EffortConnection.DbContainer);

            // Store parameters in the context
            foreach (DbParameter parameter in this.Parameters)
            {
                context.Parameters.Add(new CommandActionParameter(parameter.ParameterName, parameter.Value));
            }

            if (this.EffortTransaction != null)
            {
                context.Transaction = this.EffortTransaction.InternalTransaction;
            }

            return context;
        }
    }
}
