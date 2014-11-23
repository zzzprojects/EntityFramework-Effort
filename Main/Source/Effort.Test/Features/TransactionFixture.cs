// --------------------------------------------------------------------------------------------
// <copyright file="TransactionFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Features
{
    using System.Linq;
    using System.Transactions;
    using Effort.Test.Data.Northwind;
    using NUnit.Framework;

    [TestFixture]
    public class TransactionFixture
    {
        private NorthwindObjectContext context;

        [SetUp]
        public void Initialize()
        {
            this.context = new LocalNorthwindObjectContext();
        }

        [Test]
        public void TransactionScopeRollback()
        {
            Customer customer = new Customer();

            customer.CompanyName = "company";
            customer.CustomerID = "CUSTO";

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

        [Test]
        public void TransactionScopeCommit()
        {
            Customer customer = new Customer();

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
