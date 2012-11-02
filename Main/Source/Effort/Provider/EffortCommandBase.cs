// ----------------------------------------------------------------------------------
// <copyright file="EffortCommandBase.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;

    public abstract class EffortCommandBase : DbCommand
    {
        private EffortConnection connection;
        private EffortTransaction transaction;
        private EffortParameterCollection parameters;

        public EffortCommandBase()
        {
            this.parameters = new EffortParameterCollection();

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

        protected EffortConnection EffortConnection
        {
            get 
            { 
                return this.connection; 
            }
        }

        protected EffortTransaction EffortTransaction
        {
            get
            {
                return this.transaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get;
            set;
        }

        protected override abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);

        public override abstract int ExecuteNonQuery();

        public override abstract object ExecuteScalar();


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
    }
}
