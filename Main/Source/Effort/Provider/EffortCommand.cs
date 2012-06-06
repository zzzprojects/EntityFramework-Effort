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
    public class EffortCommand : DbCommand, ICloneable
    {
        private DbCommandTree commandTree;

        private EffortConnection connection;
        private EffortTransaction transaction;
        private EffortParameterCollection parameters;

        public EffortCommand(DbCommandTree commandtree)
        {
            this.parameters = new EffortParameterCollection();
            this.commandTree = commandtree;

            foreach (KeyValuePair<string, TypeUsage> param in commandtree.Parameters)
            {
                this.Parameters.Insert(0, new EffortParameter() { ParameterName = param.Key });
            }

        }

        public override string CommandText
        {
            get;
            set;
        }

        public override int CommandTimeout
        {
            get;
            set;
        }

        public override CommandType CommandType
        {
            get;
            set;
        }

        protected override DbParameter CreateDbParameter()
        {
            return new EffortParameter();
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get 
            {
                return this.parameters;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return this.connection;
            }
            set
            {
                // Clear connection
                if (value == null)
                {
                    this.connection = null;
                    return;
                }

                EffortConnection newConnection = value as EffortConnection;

                if (newConnection == null)
                {
                    throw new ArgumentException("Provided connection object is incompatible");
                }

                this.connection = newConnection;
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return this.transaction;
            }
            set
            {
                // Clear transaction
                if (value == null)
                {
                    this.transaction = null;
                    return;
                }

                EffortTransaction newTransaction = value as EffortTransaction;

                if (newTransaction == null)
                {
                    throw new ArgumentException("Provided transaction object is incompatible");
                }

                this.transaction = newTransaction;
                this.connection = newTransaction.Connection as EffortConnection;
            }
        }

        public override bool DesignTimeVisible
        {
            get;
            set;
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

        public override void Prepare()
        {
            
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get;
            set;
        }

        public override void Cancel()
        {

        }

        public object Clone()
        {
            return new EffortCommand(this.commandTree);
        }

        private ActionContext CreateActionContext()
        {
            ActionContext context = new ActionContext(this.connection.DbContainer);

            // Store parameters in the context
            foreach (DbParameter parameter in this.Parameters)
            {
                context.Parameters.Add(new Parameter(parameter.ParameterName, parameter.Value));
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
