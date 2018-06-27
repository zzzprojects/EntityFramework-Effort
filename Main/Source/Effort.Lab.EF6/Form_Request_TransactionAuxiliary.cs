using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity;
using System.Windows.Forms;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_TransactionAuxiliary : Form
    {
        public Form_Request_TransactionAuxiliary()
        {
            InitializeComponent();

            var connection = My.ConnectionFactory();//DbConnectionFactory.CreateTransient();

            var addToDbSet = false;

            // TEST
            using (var db = new EntityContext(connection))
            {
                var d = new Transaction();

                db.Transactions.Add(d);

                d.TransactionAuxiliary = new TransactionAuxiliary
                {
                    Transaction = d
                };


                if (addToDbSet) db.TransactionAuxiliaries.Add(d.TransactionAuxiliary);

                db.SaveChanges();

                if (d.TransactionAuxiliary == null) throw new Exception("Fails if addToDbSet");

                db.SaveChanges();

                if (d.TransactionAuxiliary == null) throw new Exception("Fails if !addToDbSet");
            }
        }

        public class EntityContext : DbContext
        {
            public EntityContext(DbConnection connection) : base(connection, true)
            {
            }

            public DbSet<TransactionAuxiliary> TransactionAuxiliaries { get; set; }
            public DbSet<Transaction> Transactions { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Transaction>().HasOptional(x => x.TransactionAuxiliary)
                    .WithRequired(x => x.Transaction);

                base.OnModelCreating(modelBuilder);
            }
        }

        public class Transaction
        {
            public int TransactionID { get; set; }

            public TransactionAuxiliary TransactionAuxiliary { get; set; }
        }

        public class TransactionAuxiliary
        {
            private Transaction _transaction;

            [Key] public int TransactionAuxiliaryID { get; set; }

            public virtual Transaction Transaction
            {
                get => _transaction;
                set
                {
                    //commenting produces test failures.
                    //_transaction = value; 
                }
            }
        }
    }
}