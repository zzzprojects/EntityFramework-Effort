using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data;
using System.Transactions;
using Effort.Test.Utils;

namespace Effort.Test
{
    [TestClass]
    public class TransactionFixture
    {
        private NorthwindEntities context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = new NorthwindEntitiesEmulated();
        }

        [TestMethod]
        public void TransactionScopeRollback()
        {
            Customers customer = new Customers();

            customer.CompanyName = "company";
            customer.CustomerID = "CUSTO";

            context.Connection.Open();

            using (TransactionScope tran = new TransactionScope())
            {
                this.context.Customers.AddObject(customer);
                this.context.SaveChanges();


                bool customerWasAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") != null;
                Assert.IsTrue(customerWasAdded);

                // Omit 'tran.Complete()' to achieve rollback 
            }

            bool customerWasNotAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") == null;
            Assert.IsTrue(customerWasNotAdded);
        }


        [TestMethod]
        public void TransactionScopeCommit()
        {
            Customers customer = new Customers();

            customer.CompanyName = "company";
            customer.CustomerID = "CUSTO";

            using (TransactionScope tran = new TransactionScope())
            {
                context.Customers.AddObject(customer);
                context.SaveChanges();

                tran.Complete();
            }

            bool customerWasAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") != null;
            
            Assert.IsTrue(customerWasAdded);
            
        }
    }
}
