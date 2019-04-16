using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using Effort.Provider;

namespace Effort.Lab.EF6
{
    public partial class Form_Request_TransactionIssue : Form
    {
        public Form_Request_TransactionIssue()
        {
            InitializeComponent();
            // Testing_connection();
            Testing_no_connection();
        }

        public void Testing_connection() // Succeeds
        {
            var context = new FakeContext(Effort.DbConnectionFactory.CreateTransient());
            var connection = (EffortConnection)context.Database.Connection;
            var trans = connection.BeginTransaction();
            using (var unit = context.Database.Connection.BeginTransaction())
            {
                Console.WriteLine(unit.IsolationLevel);
                var fake = new DummyObject
                {
                    Id = Guid.NewGuid(),
                    DummyProp = "Hello"
                };
                context.DummyObjects.Add(fake);
                context.SaveChanges();

                var results = context.DummyObjects.ToList();

                unit.Commit();
            }
        }

        public void Testing_no_connection() // Fails with Transaction Aborted
        {
            var context = new FakeContext(Effort.DbConnectionFactory.CreateTransient());
            //var test = context.DummyObjects.ToList();

            using(var transactionScope = new TransactionScope())
            //using (var unit = context.Database.BeginTransaction())
            {
               // Console.WriteLine(unit.UnderlyingTransaction.IsolationLevel);
                var fake = new DummyObject
                {
                    Id = Guid.NewGuid(),
                    DummyProp = "Hello"
                };
                context.DummyObjects.Add(fake);
                context.SaveChanges();
           

                //unit.Commit();
                
               transactionScope.Complete();
            }

            // This is the line that takes 20+ seconds to run causing the transaction to timeout
            var results = context.DummyObjects.ToList();
        }

        public class FakeContext : DbContext
        {
            public FakeContext(DbConnection existingConnection)
                : base(existingConnection, false)
            {
            }

            public DbSet<DummyObject> DummyObjects { get; set; }
        }

        public class DummyObject
        {
            [Key]
            public Guid Id { get; set; }

            public string DummyProp { get; set; }
        }
    }
}
