using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;
using System.Data.Common;

namespace Effort.Components
{
    public class EffortWrapperTransaction : DbTransaction
    {
        private EffortWrapperConnection connection;
        private System.Data.IsolationLevel isolationLevel;

        private System.Transactions.CommittableTransaction transaction;

        public EffortWrapperTransaction(EffortWrapperConnection connection, System.Data.IsolationLevel isolationLevel) 
        {
            if (System.Transactions.Transaction.Current != null)
            {
                throw new InvalidOperationException("Ambient transaction is already set");
            }

            this.connection = connection;
            this.isolationLevel = isolationLevel;

            // Initialize new ambient transaction
            System.Transactions.TransactionOptions options = new System.Transactions.TransactionOptions();
            options.IsolationLevel = TranslateIsolationLevel(isolationLevel);
            options.Timeout = new TimeSpan(0, 0, connection.ConnectionTimeout);

            this.transaction = new System.Transactions.CommittableTransaction(options);

            // If the mode of the connection is accelerator, the underlying connection has to be enlisted to this transaction
            if (connection.ProviderMode == ProviderModes.DatabaseAccelerator)
            {
                // Open the physical database connection, if it has not been yet
                if (connection.WrappedConnection.State != System.Data.ConnectionState.Open)
                {
                    connection.WrappedConnection.Open();
                }

                // Enlist the physical database connection to the ambient transaction
                connection.WrappedConnection.EnlistTransaction(transaction);
            }

            // Enlist the MMDB transaction to this transaction
            try
            {
                System.Transactions.Transaction.Current = this.transaction;

                // This will create an MMDB transaction and enlist to our ambient transaction
                bool isOpen = MMDB.Transaction.TransactionScope.IsOpen;
            }
            finally
            {
                System.Transactions.Transaction.Current = null;
            }
        }

        public override void Commit()
        {
            transaction.Commit();
        }

        protected override DbConnection DbConnection
        {
            get { return this.connection; }
        }

        public override System.Data.IsolationLevel IsolationLevel
        {
            get { return this.isolationLevel; }
        }

        public override void Rollback()
        {
            transaction.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.transaction.Dispose();
            }

            base.Dispose(disposing);
        }

        private System.Transactions.IsolationLevel TranslateIsolationLevel(System.Data.IsolationLevel isolationLevel)
        {
            switch (isolationLevel)
            {
                case System.Data.IsolationLevel.Chaos:
                    return System.Transactions.IsolationLevel.Chaos;
                case System.Data.IsolationLevel.ReadCommitted:
                    return System.Transactions.IsolationLevel.ReadCommitted;
                case System.Data.IsolationLevel.ReadUncommitted:
                    return System.Transactions.IsolationLevel.ReadUncommitted;
                case System.Data.IsolationLevel.RepeatableRead:
                    return System.Transactions.IsolationLevel.RepeatableRead;
                case System.Data.IsolationLevel.Serializable:
                    return System.Transactions.IsolationLevel.Serializable;
                case System.Data.IsolationLevel.Snapshot:
                    return System.Transactions.IsolationLevel.Snapshot;
                case System.Data.IsolationLevel.Unspecified:
                    return System.Transactions.IsolationLevel.Unspecified;
                default:
                    throw new ArgumentException("Unknown isolation level.", "isolationLevel");
            }
        }
    }
}
