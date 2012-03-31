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
    public class ExpressionFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

        }

        [TestMethod]
        public void Addition()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID + 3 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Substraction()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID - 3 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Negation()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where - (emp.EmployeeID - 5) == 2
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Multiply()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID * 2 == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Divide()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID / 2 == 2
                    select emp

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void ComplexArithmetic()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where  - (emp.EmployeeID * 3) / 2 + 5 == 1
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID == 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GreaterThan()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID > 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GreaterThanOrEquals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID >= 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SmallerThan()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID < 4
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SmallerThanOrEquals()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID <= 4
                    select emp

            );

            Assert.IsTrue(result);
        }




    }
}
