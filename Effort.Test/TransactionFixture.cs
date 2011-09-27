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
            ////this.context = new NorthwindEntities("name=NorthwindEntities");
            this.context = new NorthwindEntitiesWrapped("name=NorthwindEntities");
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
                context.Customers.AddObject(customer);
                context.SaveChanges();


                bool customerWasAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") != null;
                Assert.IsTrue(customerWasAdded);

                // Omit to achieve rollback: 
                ////tran.Complete();
            }

            bool customerWasNotAdded = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") == null;
            Assert.IsTrue(customerWasNotAdded);
        }


        [TestMethod]
        public void TransactionScopeCommit()
        {
            // -------------------------------------------------------
            //                      Cleanup
            // -------------------------------------------------------
            
            Customers delete = context.Customers.SingleOrDefault(c => c.CustomerID == "CUSTO");

            if (delete != null)
            {
                context.Customers.DeleteObject(delete);
                context.SaveChanges();
            }

            // -------------------------------------------------------
            //                      Start
            // -------------------------------------------------------
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

            // -------------------------------------------------------
            //                      Cleanup
            // -------------------------------------------------------
            context.Customers.DeleteObject(customer);
            context.SaveChanges();

            bool customerWasDeleted = context.Customers.FirstOrDefault(c => c.CustomerID == "CUSTO") == null;

            Assert.IsTrue(customerWasDeleted);
            
        }
    }
}
