using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Effort.Provider
{
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
