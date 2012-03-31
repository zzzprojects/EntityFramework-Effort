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

using System.Linq;
using Effort.Test.Data;
using Effort.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class AggregationFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

        }

        [TestMethod]
        public void Count()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Customers.Count()

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NestedCount()
        {
            bool result = this.runtime.Execute(

                context =>
                    from cus in context.Customers
                    select new { id = cus.CustomerID, count = cus.Orders.Count() }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GroupByCount()
        {
            bool result = this.runtime.Execute(

                context =>
                    from ord in context.Orders
                    group ord by ord.CustomerID into ordg
                    select new { id = ordg.Key, count = ordg.Count() }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Average()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Orders.Average(o => o.Freight)

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NestedAverage()
        {
            bool result = this.runtime.Execute(

                context =>
                    from cus in context.Customers
                    select new { id = cus.CustomerID, avg = cus.Orders.Average(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GroupByAverage()
        {
            bool result = this.runtime.Execute(

                context =>
                    from ord in context.Orders
                    group ord by ord.CustomerID into ordg
                    select new { id = ordg.Key, avg = ordg.Average(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Sum()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Orders.Sum(o => o.Freight)

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void NestedSum()
        {
            bool result = this.runtime.Execute(

                context =>
                    from cus in context.Customers
                    //SQL Provider returns sum = NULL, if the underlying collection is empty
                    select new { id = cus.CustomerID, sum = cus.Orders.Sum(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NullCollectionSum()
        {
            bool result = this.runtime.Execute(

                context =>
                    (
                    from ord in context.Orders
                    where ord.Freight > 10000
                    select ord
                    )
                    .Sum(o => o.Freight)

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Max()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Orders.Max(o => o.Freight)

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void NestedMax()
        {
            bool result = this.runtime.Execute(

                context =>
                    from cus in context.Customers
                    select new { id = cus.CustomerID, max = cus.Orders.Max(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GroupByMax()
        {
            bool result = this.runtime.Execute(

                context =>
                    from ord in context.Orders
                    group ord by ord.CustomerID into ordg
                    select new { id = ordg.Key, max = ordg.Max(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Min()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Orders.Min(o => o.Freight)

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void NestedMin()
        {
            bool result = this.runtime.Execute(

                context =>
                    from cus in context.Customers
                    select new { id = cus.CustomerID, min = cus.Orders.Min(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GroupByMin()
        {
            bool result = this.runtime.Execute(

                context =>
                    from ord in context.Orders
                    group ord by ord.CustomerID into ordg
                    select new { id = ordg.Key, min = ordg.Min(o => o.Freight) }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MultipleGroupByAggregation()
        {
            bool result = this.runtime.Execute(

                context =>
                    from ord in context.Orders
                    group ord by ord.CustomerID into ordg
                    select new 
                    { 
                        id = ordg.Key,
                     
                        count = ordg.Count(),
                        avg = ordg.Average(o => o.Freight),
                        max = ordg.Max(o => o.Freight),
                        min = ordg.Min(o => o.Freight),
                        sum = ordg.Sum(o => o.Freight)
                    }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MultipleNestedAggregation()
        {
            bool result = this.runtime.Execute(
                
                context =>
                    from cus in context.Customers
                    select new
                    {
                        id = cus.CustomerID,

                        count = cus.Orders.Count(),
                        avg = cus.Orders.Average(o => o.Freight),
                        max = cus.Orders.Max(o => o.Freight),
                        min = cus.Orders.Min(o => o.Freight),
                        //SQL Provider returns sum = NULL, if the underlying collection is empty
                        sum = cus.Orders.Sum(o => o.Freight)
                    }

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Having()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Orders

                    .GroupBy(o => o.CustomerID)
                    .Where(o => o.Average(x => x.Freight) > 1)

            );

            Assert.IsTrue(result);
        }



    }
}
